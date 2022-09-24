using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CarinaStudio.MacOS.ObjectiveC
{
    /// <summary>
    /// Class of Objective-C.
    /// </summary>
    public sealed unsafe class Class
    {
        // Native symbols.
        [DllImport(NativeLibraryNames.ObjectiveC)]
        static extern IntPtr class_getClassVariable(IntPtr cls, string name);
        [DllImport(NativeLibraryNames.ObjectiveC)]
        static extern IntPtr class_getInstanceVariable(IntPtr cls, string name);
        [DllImport(NativeLibraryNames.ObjectiveC)]
        static extern sbyte* class_getName(IntPtr cls);
        [DllImport(NativeLibraryNames.ObjectiveC)]
        static extern IntPtr class_getProperty(IntPtr cls, string name);
        [DllImport(NativeLibraryNames.ObjectiveC)]
        static extern IntPtr class_getSuperclass(IntPtr cls);
        [DllImport(NativeLibraryNames.ObjectiveC)]
        static extern IntPtr objc_getClass(string name);
        [DllImport(NativeLibraryNames.ObjectiveC)]
        static extern IntPtr property_copyAttributeValue(IntPtr property, string attributeName);
        [DllImport(NativeLibraryNames.ObjectiveC, EntryPoint = NSObject.SendMessageEntryPointName)]
        static extern IntPtr SendMessageIntPtr(IntPtr target, IntPtr selector);


        // Static fields.
        static readonly Selector AllocSelector = Selector.GetOrCreate("alloc");
        static readonly IDictionary<IntPtr, Class> CachedClassesByHandle = new ConcurrentDictionary<IntPtr, Class>();
        static readonly IDictionary<string, Class> CachedClassesByName = new ConcurrentDictionary<string, Class>();
        static readonly IDictionary<string, MemberDescriptor> CachedIVars = new ConcurrentDictionary<string, MemberDescriptor>();
        static readonly IDictionary<string, PropertyDescriptor> CachedProperties = new ConcurrentDictionary<string, PropertyDescriptor>();


        // Fields.
        volatile bool isRootClass;
        volatile Class? superClass;


        // Constructor.
        Class(IntPtr handle, string name)
        {
            this.Handle = handle;
            this.Name = name;
        }


        /// <summary>
        /// Allocate memory for new instance with this class.
        /// </summary>
        /// <returns>Handle of allocated instance.</returns>
        public IntPtr Allocate() =>
            SendMessageIntPtr(this.Handle, AllocSelector.Handle);


        /// <summary>
        /// Find the class with given name.
        /// </summary>
        /// <param name="name">Name of class.</param>
        /// <returns>Class with given name, or Null if class cannot be found.</returns>
        public static Class? GetClass(string name)
        {
            if (CachedClassesByName.TryGetValue(name, out var nsClass))
                return nsClass;
            if (Platform.IsNotMacOS)
                return null;
            var handle = objc_getClass(name);
            if (handle != IntPtr.Zero)
            {
                return new Class(handle, name).Also(it => 
                {
                    CachedClassesByName.TryAdd(name, it);
                    CachedClassesByHandle.TryAdd(handle, it);
                });
            }
            return null;
        }


        /// <summary>
        /// Get handle of class.
        /// </summary>
        public IntPtr Handle { get; }


        /// <summary>
        /// Get name of class.
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// Get super class.
        /// </summary>
        public Class? SuperClass
        {
            get
            {
                return this.superClass 
                    ?? (this.isRootClass 
                        ? null
                        : class_getSuperclass(this.Handle).Let(it =>
                        {
                            if (it == IntPtr.Zero)
                            {
                                this.isRootClass = true;
                                return null;
                            }
                            this.superClass = Wrap(it);
                            return this.superClass;
                        }));
            }
        }


        /// <inheritdoc/>
        public override string ToString() =>
            this.Name;
        

        /// <summary>
        /// Try finding instance variable of class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="ivar">Descriptor of instance variable.</param>
        /// <returns>True if variable found.</returns>
        public unsafe bool TryFindInstanceVriable(string name, out MemberDescriptor? ivar)
        {
            if (CachedIVars.TryGetValue(name, out ivar))
                return true;
            var handle = class_getInstanceVariable(this.Handle, name);
            if (handle == IntPtr.Zero)
            {
                ivar = null;
                return false;
            }
            ivar = new MemberDescriptor(handle, name).Also(it => CachedIVars.TryAdd(name, it));
            return true;
        }
        

        /// <summary>
        /// Try finding property of class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="property">Descriptor of found property.</param>
        /// <returns>True if property found.</returns>
        public unsafe bool TryFindProperty(string name, out PropertyDescriptor? property)
        {
            if (CachedProperties.TryGetValue(name, out property))
                return true;
            var handle = class_getProperty(this.Handle, name);
            if (handle == IntPtr.Zero)
            {
                property = null;
                return false;
            }
            var getter = property_copyAttributeValue(handle, "G").Let(it =>
            {
                if (it != IntPtr.Zero)
                {
                    var selector = Selector.GetOrCreate(new string((sbyte*)it));
                    NativeMemory.Free((void*)it);
                    return selector;
                }
                return Selector.GetOrCreateUid(name);
            });
            var isReadOnly = property_copyAttributeValue(handle, "R").Let(it =>
            {
                if (it != IntPtr.Zero)
                {
                    NativeMemory.Free((void*)it);
                    return true;
                }
                return false;
            });
            var setter = isReadOnly ? null : property_copyAttributeValue(handle, "S").Let(it =>
            {
                if (it != IntPtr.Zero)
                {
                    var selector = Selector.GetOrCreate(new string((sbyte*)it));
                    NativeMemory.Free((void*)it);
                    return selector;
                }
                return Selector.GetOrCreateUid(name);
            });
            property = new PropertyDescriptor(handle, name, getter, setter).Also(it => CachedProperties.TryAdd(name, it));
            return true;
        }
        

        /// <summary>
        /// Wrap native instance as <see cref="Class"/>.
        /// </summary>
        /// <param name="handle">Handle of instance.</param>
        /// <returns><see cref="Class"/>.</returns>
        public static unsafe Class Wrap(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentException("Handle of instance cannot be null.");
            if (CachedClassesByHandle.TryGetValue(handle, out var nsClass))
                return nsClass;
            var namePtr = class_getName(handle);
            if (namePtr == null)
                throw new ArgumentException("Unable to get name of class.");
            var name = new string(namePtr);
            if (CachedClassesByName.TryGetValue(name, out nsClass))
                return nsClass;
            return new Class(handle, name).Also(it => 
            {
                CachedClassesByName.TryAdd(name, it);
                CachedClassesByHandle.TryAdd(handle, it);
            });
        }
    }
}