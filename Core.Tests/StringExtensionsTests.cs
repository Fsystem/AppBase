using NUnit.Framework;

namespace CarinaStudio;

/// <summary>
/// Tests of <see cref="StringExtensions"/>.
/// </summary>
[TestFixture]
public class StringExtensionsTests
{
    /// <summary>
    /// Test for <see cref="StringExtensions.GetFirstLine"/>
    /// </summary>
    [Test]
    public void GetFirstLineTest()
    {
        // empty string
        var s1 = "";
        var s2 = s1.GetFirstLine();
        Assert.AreEqual("", s2);
        Assert.IsTrue(ReferenceEquals(s1, s2));
        
        // normal strings
        s1 = "0";
        s2 = s1.GetFirstLine();
        Assert.AreEqual("0", s2);
        Assert.IsTrue(ReferenceEquals(s1, s2));
        s1 = "01234";
        s2 = s1.GetFirstLine();
        Assert.AreEqual("01234", s2);
        Assert.IsTrue(ReferenceEquals(s1, s2));
        s1 = "0\n12";
        s2 = s1.GetFirstLine();
        Assert.AreEqual("0", s2);
        s1 = "01234\n56";
        s2 = s1.GetFirstLine();
        Assert.AreEqual("01234", s2);
        s1 = "01234\r\n56";
        s2 = s1.GetFirstLine();
        Assert.AreEqual("01234", s2);
        s1 = "01234\r\n56";
        s2 = s1.GetFirstLine(true);
        Assert.AreEqual("01234\r", s2);
        s1 = "\r01234\n56";
        s2 = s1.GetFirstLine();
        Assert.AreEqual("01234", s2);
        s1 = "\r01234\n56";
        s2 = s1.GetFirstLine(true);
        Assert.AreEqual("\r01234", s2);
        s1 = "0\r1234\n56";
        s2 = s1.GetFirstLine();
        Assert.AreEqual("01234", s2);
        s1 = "0\r1234\n56";
        s2 = s1.GetFirstLine(true);
        Assert.AreEqual("0\r1234", s2);
        
        // strings with leading line breaks
        s1 = "\n0";
        s2 = s1.GetFirstLine();
        Assert.AreEqual("", s2);

        // strings with trailing line breaks
        s1 = "0\n";
        s2 = s1.GetFirstLine();
        Assert.AreEqual("0", s2);
        s1 = "01234\r\n";
        s2 = s1.GetFirstLine();
        Assert.AreEqual("01234", s2);
        s1 = "01234\r\n";
        s2 = s1.GetFirstLine(true);
        Assert.AreEqual("01234\r", s2);

        // strings with line breaks only
        s1 = "\n";
        s2 = s1.GetFirstLine();
        Assert.AreEqual("", s2);
    }
    
    
    /// <summary>
    /// Test for <see cref="StringExtensions.GetLastLine"/>
    /// </summary>
    [Test]
    public void GetLastLineTest()
    {
        // empty string
        var s1 = "";
        var s2 = s1.GetLastLine();
        Assert.AreEqual("", s2);
        Assert.IsTrue(ReferenceEquals(s1, s2));
        
        // normal strings
        s1 = "0";
        s2 = s1.GetLastLine();
        Assert.AreEqual("0", s2);
        Assert.IsTrue(ReferenceEquals(s1, s2));
        s1 = "01234";
        s2 = s1.GetLastLine();
        Assert.AreEqual("01234", s2);
        Assert.IsTrue(ReferenceEquals(s1, s2));
        s1 = "0\n12";
        s2 = s1.GetLastLine();
        Assert.AreEqual("12", s2);
        s1 = "01234\n56";
        s2 = s1.GetLastLine();
        Assert.AreEqual("56", s2);
        s1 = "01234\n\r56";
        s2 = s1.GetLastLine();
        Assert.AreEqual("56", s2);
        s1 = "01234\n\r56";
        s2 = s1.GetLastLine(true);
        Assert.AreEqual("\r56", s2);
        s1 = "012\n3456\r";
        s2 = s1.GetLastLine();
        Assert.AreEqual("3456", s2);
        s1 = "012\n3456\r";
        s2 = s1.GetLastLine(true);
        Assert.AreEqual("3456\r", s2);
        s1 = "012\n34\r56";
        s2 = s1.GetLastLine();
        Assert.AreEqual("3456", s2);
        s1 = "012\n34\r56";
        s2 = s1.GetLastLine(true);
        Assert.AreEqual("34\r56", s2);

        // strings with leading line breaks
        s1 = "\n0";
        s2 = s1.GetLastLine();
        Assert.AreEqual("0", s2);
        s1 = "\n\r01234";
        s2 = s1.GetLastLine();
        Assert.AreEqual("01234", s2);
        s1 = "\n\r01234";
        s2 = s1.GetLastLine(true);
        Assert.AreEqual("\r01234", s2);

        // strings with trailing line breaks
        s1 = "0\n";
        s2 = s1.GetLastLine();
        Assert.AreEqual("", s2);

        // strings with line breaks only
        s1 = "\n";
        s2 = s1.GetLastLine();
        Assert.AreEqual("", s2);
    }
    
    
    /// <summary>
    /// Test for <see cref="StringExtensions.HasMultipleLines"/>
    /// </summary>
    [Test]
    public void HasMultipleLinesTest()
    {
        // empty string
        var s = "";
        Assert.AreEqual(false, s.HasMultipleLines());

        // normal strings
        s = "0";
        Assert.AreEqual(false, s.HasMultipleLines());
        s = "01234";
        Assert.AreEqual(false, s.HasMultipleLines());
        s = "0\n12";
        Assert.AreEqual(true, s.HasMultipleLines());
        s = "0\n12\n345";
        Assert.AreEqual(true, s.HasMultipleLines());

        // strings with leading line breaks
        s = "\n0";
        Assert.AreEqual(true, s.HasMultipleLines());
        s = "\n\n0";
        Assert.AreEqual(true, s.HasMultipleLines());

        // strings with trailing line breaks
        s = "0\n";
        Assert.AreEqual(true, s.HasMultipleLines());
        s = "0\n\n";
        Assert.AreEqual(true, s.HasMultipleLines());

        // strings with line breaks only
        s = "\n";
        Assert.AreEqual(true, s.HasMultipleLines());
        s = "\n\n";
        Assert.AreEqual(true, s.HasMultipleLines());
    }
    
    
    /// <summary>
    /// Test for <see cref="StringExtensions.LineCount"/>
    /// </summary>
    [Test]
    public void LineCountTest()
    {
        // empty string
        var s = "";
        Assert.AreEqual(1, s.LineCount());

        // normal strings
        s = "0";
        Assert.AreEqual(1, s.LineCount());
        s = "01234";
        Assert.AreEqual(1, s.LineCount());
        s = "0\n12";
        Assert.AreEqual(2, s.LineCount());
        s = "0\n12\n345";
        Assert.AreEqual(3, s.LineCount());

        // strings with leading line breaks
        s = "\n0";
        Assert.AreEqual(2, s.LineCount());
        s = "\n\n0";
        Assert.AreEqual(3, s.LineCount());
        s = "\n0\n12";
        Assert.AreEqual(3, s.LineCount());

        // strings with trailing line breaks
        s = "0\n";
        Assert.AreEqual(2, s.LineCount());
        s = "0\n\n";
        Assert.AreEqual(3, s.LineCount());
        s = "0\n12\n";
        Assert.AreEqual(3, s.LineCount());

        // strings with line breaks only
        s = "\n";
        Assert.AreEqual(2, s.LineCount());
        s = "\n\n";
        Assert.AreEqual(3, s.LineCount());
    }
    
    
    /// <summary>
    /// Test for <see cref="StringExtensions.RemoveLineBreaks"/>
    /// </summary>
    [Test]
    public void RemoveLineBreaksTest()
    {
        // empty string
        var s1 = "";
        var s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("", s2);
        Assert.IsTrue(ReferenceEquals(s1, s2));
        
        // normal strings
        s1 = "0";
        s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("0", s2);
        Assert.IsTrue(ReferenceEquals(s1, s2));
        s1 = "0\n1";
        s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("01", s2);
        s1 = "0\n\r1";
        s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("01", s2);
        s1 = "0\n\r1";
        s2 = s1.RemoveLineBreaks(false);
        Assert.AreEqual("0\r1", s2);
        s1 = "0\n12\r345";
        s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("012345", s2);
        s1 = "0\n12\r345";
        s2 = s1.RemoveLineBreaks(false);
        Assert.AreEqual("012\r345", s2);

        // strings with leading line breaks
        s1 = "\n0";
        s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("0", s2);
        s1 = "\r\n0";
        s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("0", s2);
        s1 = "\r\n0";
        s2 = s1.RemoveLineBreaks(false);
        Assert.AreEqual("\r0", s2);
        s1 = "\n01234";
        s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("01234", s2);

        // strings with trailing line breaks
        s1 = "0\n";
        s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("0", s2);
        s1 = "0\r\n";
        s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("0", s2);
        s1 = "0\r\n";
        s2 = s1.RemoveLineBreaks(false);
        Assert.AreEqual("0\r", s2);
        s1 = "01234\n";
        s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("01234", s2);
        
        // strings with line breaks only
        s1 = "\n";
        s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("", s2);
        s1 = "\r";
        s2 = s1.RemoveLineBreaks(false);
        Assert.AreEqual("\r", s2);
        s1 = "\n\r";
        s2 = s1.RemoveLineBreaks();
        Assert.AreEqual("", s2);
        s1 = "\n\r";
        s2 = s1.RemoveLineBreaks(false);
        Assert.AreEqual("\r", s2);
    }
}