using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

namespace Hypothesist.Tests.Experiments;

public static class AtLeast
{
    [Fact]
    public static async Task Valid()
    {
        var hypothesis = Hypothesis
            .For<string>()
            .AtLeast(2, x => x == "a");

        await hypothesis.Test("a");
        await hypothesis.Test("a");
            
        await hypothesis
            .Validate(20.Minutes());
    }
        
    [Fact]
    public static async Task None()
    {
        var hypothesis = Hypothesis
            .For<string>()
            .AtLeast(2, x => x == "a");

        var act = () => hypothesis.Validate(1.Seconds());
        await act.Should()
            .ThrowAsync<HypothesisInvalidException<string>>()
            .WithMessage("*at least 2*");
    }
        
    [Fact]
    public static async Task Less()
    {
        var hypothesis = Hypothesis
            .For<string>()
            .AtLeast(2, x => x == "a");
            
        await hypothesis.Test("a");
            
        var act = () => hypothesis.Validate(1.Seconds());
        var ex = await act.Should()
            .ThrowAsync<HypothesisInvalidException<string>>();

        ex.Which
            .Matched
            .Should()
            .BeEquivalentTo("a");
    }
        
    [Fact]
    public static async Task More()
    {
        var hypothesis = Hypothesis
            .For<string>()
            .All(x => x != "b") // combined with non-breaking observer
            .AtLeast(2, x => x == "a");
            
        await hypothesis.Test("a");
        await hypothesis.Test("a");
        await hypothesis.Test("a");
            
        await hypothesis.Validate(1.Seconds());
    }
        
    [Fact]
    public static async Task Unmatched()
    {
        var hypothesis = Hypothesis
            .For<string>()
            .AtLeast(2, x => x == "a");
            
        await hypothesis.Test("b");
            
        var act = () => hypothesis.Validate(1.Seconds());
        var ex = await act.Should()
            .ThrowAsync<HypothesisInvalidException<string>>();

        ex.Which
            .Unmatched
            .Should()
            .BeEquivalentTo("b");
    }
}