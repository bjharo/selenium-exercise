using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Ui.Browser;

public static class SeleniumExtensions
{
    private static TimeSpan DefaultExplicitTimeout => TimeSpan.FromSeconds(60);

    public static IWebElement GetElement(this ISearchContext context, By Selector, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(context, actualTimeout).Until(c => c.FindElement(Selector));
        }
        catch (Exception ex) when (ex is WebDriverTimeoutException or WebDriverException)
        {
            if (ex is WebDriverTimeoutException
                || ex.Message.Contains($"timed out after {actualTimeout.TotalSeconds} seconds"))
            {
                throw new NoSuchElementException($"Unable to find element with Selector {Selector.ToString()}.", ex);
            }

            throw;
        }
    }

    public static bool WaitForElementToBeDisplayed(this ISearchContext driver, By Selector, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(driver, actualTimeout).Until(d => d.FindElement(Selector).Displayed);
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public static void ClearAndSendKeys(this IWebElement element, string newText)
    {
        if (OperatingSystem.IsMacOS())
        {
            element.SendKeys(Keys.Command + "a");
        }
        else
        {
            element.SendKeys(Keys.Control + "a");
        }

        element.SendKeys(Keys.Backspace + newText);
    }

    public static bool WaitForElementToNotBeDisplayed(this ISearchContext driver, By Selector, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(driver, actualTimeout).Until(e =>
            {
                try
                {
                    var isDisplayed = e.FindElement(Selector).Displayed;
                    return !isDisplayed;
                }
                catch (Exception ex) when (ex is NoSuchElementException or StaleElementReferenceException)
                {
                    return true;
                }
            });
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public static bool WaitForElementToHaveText(this ISearchContext driver, By Selector, string text, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(driver, actualTimeout).Until(d => d.FindElement(Selector).Text.Equals(text));
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
    
    public static bool WaitForElementToContainText(this ISearchContext driver, By Selector, string text, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(driver, actualTimeout).Until(d => d.FindElement(Selector).Text.Contains(text));
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
    
    public static bool WaitForElementToHaveValue(this ISearchContext driver, By Selector, string text, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(driver, actualTimeout).Until(d => d.FindElement(Selector).GetAttribute("value").Equals(text));
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public static bool WaitForElementToBeEnabled(this ISearchContext driver, By Selector, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(driver, actualTimeout).Until(d => d.FindElement(Selector)).Enabled;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    private static DefaultWait<ISearchContext> CreateWait(ISearchContext driver, TimeSpan timeout)
    {
        var wait = new DefaultWait<ISearchContext>(driver) {   
            Timeout = timeout,
            PollingInterval = TimeSpan.FromMilliseconds(250)
        };

        wait.IgnoreExceptionTypes(new[] { 
            typeof(StaleElementReferenceException), 
            typeof(NotFoundException), 
            typeof(InvalidOperationException)
        });

        return wait;
    }
}