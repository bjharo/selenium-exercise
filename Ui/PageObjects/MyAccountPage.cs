using OpenQA.Selenium;
using Ui.Browser;

namespace Ui.PageObjects;

public class MyAccountPage : Page
{
    private By NameAndEmailInfoSelector => By.CssSelector(".block-dashboard-info .box-content");
    
    public MyAccountPage(IWebDriver driver) : base(driver)
    { }

    public bool IsLoaded()
    {
        return Driver.WaitForElementToHaveText(PageTitleSelector, "My Account")
               && Driver.WaitForElementToBeDisplayed(NameAndEmailInfoSelector);
    }

    public string GetNameAndEmail()
    {
        return Driver.GetElement(NameAndEmailInfoSelector).Text;
    }
}