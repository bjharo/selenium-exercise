using OpenQA.Selenium;
using Ui.Browser;

namespace Ui.PageObjects;

public class CartPage : Page
{
    private By CheckoutBtnSelector => By.CssSelector("[data-role = 'proceed-to-checkout']");
    
    public CartPage(IWebDriver driver) : base(driver)
    { }

    public bool IsLoaded()
    {
        return Driver.WaitForElementToHaveText(PageTitleSelector, "Shopping Cart")
               && Driver.WaitForElementToStopAnimating(CheckoutBtnSelector);
    }

    public (bool productOnPage, CartItemWidget? itemWidget) GetCartItem(string productName)
    {
        var selector =
            By.XPath(
                $"//tr[contains(@class, 'item-info')][contains(normalize-space(.), '{productName}')]");

        try
        {
            return (true, new CartItemWidget(Driver, Driver.GetElement(selector, TimeSpan.FromSeconds(5))));
        }
        catch (NoSuchElementException)
        {
            return (false, null);
        }
    }

    public void ProceedToCheckout()
    {
        Driver.GetElement(CheckoutBtnSelector).Click();
    }
}