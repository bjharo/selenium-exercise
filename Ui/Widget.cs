using OpenQA.Selenium;

namespace Ui;

public abstract class Widget
{
    protected IWebDriver Driver { get; init; }

    protected Widget(IWebDriver driver)
    {
        Driver = driver;
    }
}