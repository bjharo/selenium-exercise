using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Ui.Browser;

public static class SeleniumExtensions
{
    private static TimeSpan DefaultExplicitTimeout => TimeSpan.FromSeconds(60);

    public static IWebElement GetElement(this ISearchContext context, By selector, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(context, actualTimeout).Until(c => c.FindElement(selector));
        }
        catch (Exception ex) when (ex is WebDriverTimeoutException or WebDriverException)
        {
            if (ex is WebDriverTimeoutException
                || ex.Message.Contains($"timed out after {actualTimeout.TotalSeconds} seconds"))
            {
                throw new NoSuchElementException($"Unable to find element with Selector {selector.ToString()}.", ex);
            }

            throw;
        }
    }

    public static bool WaitForElementToBeDisplayed(this ISearchContext driver, By selector, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(driver, actualTimeout).Until(d => d.FindElement(selector).Displayed);
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

    public static bool WaitForElementToNotBeDisplayed(this ISearchContext driver, By selector, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(driver, actualTimeout).Until(e =>
            {
                try
                {
                    var isDisplayed = e.FindElement(selector).Displayed;
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

    public static bool WaitForElementToHaveText(this ISearchContext driver, By selector, string text, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(driver, actualTimeout).Until(d => d.FindElement(selector).Text.Equals(text));
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
    
    public static bool WaitForElementToContainText(this ISearchContext driver, By selector, string text, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(driver, actualTimeout).Until(d => d.FindElement(selector).Text.Contains(text));
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
    
    public static bool WaitForElementToHaveValue(this ISearchContext driver, By selector, string text, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(driver, actualTimeout).Until(d => d.FindElement(selector).GetAttribute("value").Equals(text));
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public static bool WaitForElementToBeEnabled(this ISearchContext driver, By selector, TimeSpan? timeout = null)
    {
        var actualTimeout = timeout ?? DefaultExplicitTimeout;

        try
        {
            return CreateWait(driver, actualTimeout).Until(d => d.FindElement(selector)).Enabled;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
    
    public static bool WaitForElementToStopAnimating(this ISearchContext driver, By selector, Int32 timeoutInMs = 5000, Int32 waitTimeInMs = 250)
    {
        var doneMoving = false;

        var prevCss = string.Empty;
        var prevStyle = string.Empty;
        var cumulative = 0;

        for (var count = 0; count < 10; count++)
        {
            try
            {
                var css = driver.GetElement(selector).GetAttribute("class");
                var style = driver.GetElement(selector).GetAttribute("style");

                if (css.Equals(prevCss) || style.Equals(prevStyle))
                {
                    cumulative++;
                    if (cumulative > 2)
                    {
                        doneMoving = true;
                        break;
                    }
                }
                else if (cumulative > 0)
                {
                    cumulative--;
                }

                prevCss = css;
                prevStyle = style;
            }
            catch (StaleElementReferenceException) { }

            Thread.Sleep(waitTimeInMs);
        }

        return doneMoving;
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