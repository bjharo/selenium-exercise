using OpenQA.Selenium;
using Ui.Browser;

namespace Ui.PageObjects;

public class ThankYouPage : Page
{
    private By ContinueShoppingBtnSelector => By.CssSelector("a.action.continue");
    private By CheckoutSuccessSelector => By.CssSelector(".checkout-success");
    
    public ThankYouPage(IWebDriver driver) : base(driver)
    { }

    public bool IsLoaded()
    {
        return Driver.WaitForElementToHaveText(PageTitleSelector, "Thank you for your purchase!")
               && Driver.WaitForElementToBeEnabled(ContinueShoppingBtnSelector);
    }

    public string GetCheckoutSuccessMessage()
    {
        return Driver.GetElement(CheckoutSuccessSelector).Text;
    }
}