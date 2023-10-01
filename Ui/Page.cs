using OpenQA.Selenium;
using Ui.Browser;

namespace Ui;

public abstract class Page
{
    protected By PageTitleSelector => By.CssSelector("[data-ui-id = 'page-title-wrapper']");
    
    protected IWebDriver Driver { get; init; }

    protected Page(IWebDriver driver)
    {
        Driver = driver;
    }

    public string GetPageTitle()
    {
        return Driver.GetElement(PageTitleSelector).Text;
    }
}