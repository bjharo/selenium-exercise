using Ardalis.GuardClauses;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Ui.Browser;

namespace Ui.PageObjects;

public class SectionsNavWidget : Widget
{
    private string SectionSelectorBase => "//div[contains(@class, 'nav-sections')]//li[contains(., '{0}')]";
    
    public SectionsNavWidget(IWebDriver driver) : base(driver)
    { }

    public bool IsLoaded()
    {
        return Driver.WaitForElementToBeDisplayed(By.XPath(string.Format(SectionSelectorBase, "Women"))) &&
               Driver.WaitForElementToBeDisplayed(By.XPath(string.Format(SectionSelectorBase, "Sale")));
    }

    /// <summary>
    /// Navigates the section menus and clicks on the appropriate one
    /// </summary>
    /// <param name="sections">A list of sections. The list must be in order that a user would hover over them and
    /// the last item in the list will be clicked on.</param>
    public void NavigateToSection(params string[] sections)
    {
        //"//div[contains(@class, 'nav-sections')]//li[contains(@class, 'level0')][contains(., 'Men')]//li[contains(., 'Bottoms')]//li[contains(., 'Pants')]"
        Guard.Against.InvalidInput(sections, nameof(sections), strings => strings.Length > 0);

        var selector = string.Empty;
        
        for (var count = 0; count < sections.Length; count++)
        {
            selector = count is 0
                ? string.Format(SectionSelectorBase, sections[count])
                : selector + $"//li[contains(., '{sections[count]}')]";
            
            if (count == sections.Length - 1)
            {
                Driver.GetElement(By.XPath(selector)).Click();
            }
            else
            {
                var sectionElement = Driver.GetElement(By.XPath(selector));
                var action = new Actions(Driver);
                action.MoveToElement(sectionElement).Perform();
            }
        }
    }
}