using FluentAssertions;
using QuranSchool.Domain.Abstractions;
using Xunit;

namespace QuranSchool.UnitTests.Domain.Abstractions;

public class PagedListTests
{
    [Fact]
    public void PagedList_ShouldSetProperties()
    {
        var items = new List<int> { 1, 2, 3 };
        var pagedList = new PagedList<int>(items, 1, 10, 3);

        pagedList.Items.Should().BeEquivalentTo(items);
        pagedList.Page.Should().Be(1);
        pagedList.PageSize.Should().Be(10);
        pagedList.TotalCount.Should().Be(3);
    }

    [Fact]
    public void HasNextPage_ShouldBeTrue_WhenMoreItemsExist()
    {
        var pagedList = new PagedList<int>(new List<int> { 1 }, 1, 1, 2);
        pagedList.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_ShouldBeFalse_WhenNoMoreItems()
    {
        var pagedList = new PagedList<int>(new List<int> { 1 }, 2, 1, 2);
        pagedList.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_ShouldBeTrue_WhenNotOnFirstPage()
    {
        var pagedList = new PagedList<int>(new List<int> { 1 }, 2, 1, 2);
        pagedList.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void HasPreviousPage_ShouldBeFalse_WhenOnFirstPage()
    {
        var pagedList = new PagedList<int>(new List<int> { 1 }, 1, 1, 2);
        pagedList.HasPreviousPage.Should().BeFalse();
    }
}
