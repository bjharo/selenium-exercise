using OpenQA.Selenium;
using Ui.Browser;

namespace Ui.PageObjects;

public class PaymentMethodPage : Page
{
    private By StepTitleSelector => By.CssSelector(".payment-group .step-title");
    private By PlaceOrderBtnSelector => By.CssSelector("#checkout button.checkout > span");
    private By CartSubtotalSelector => By.CssSelector(".sub .price");
    private By ShippingAmountSelector => By.CssSelector(".shipping .price");
    private By OrderTotalSelector => By.CssSelector(".grand .price");
    private By BillingAddressDetailsSelector => By.CssSelector(".billing-address-details");
    private By ShipToSelector => By.CssSelector(".ship-to .shipping-information-content");
    private By ShipMethodSelector => By.CssSelector(".ship-via .value");
    
    public PaymentMethodPage(IWebDriver driver) : base(driver)
    { }

    public bool IsLoaded()
    {
        return Driver.WaitForElementToHaveText(StepTitleSelector, "Payment Method")
               && Driver.WaitForElementToBeEnabled(PlaceOrderBtnSelector)
               && Driver.WaitForElementToNotBeDisplayed(By.CssSelector(".loader"));
    }

    public string GetBillingAddressDetails()
    {
        return Driver.GetElement(BillingAddressDetailsSelector).Text;
    }

    public string GetShipToDetails()
    {
        return Driver.GetElement(ShipToSelector).Text;
    }

    public string GetShipViaDetails()
    {
        return Driver.GetElement(ShipMethodSelector).Text;
    }

    public string GetSubTotal()
    {
        return Driver.GetElement(CartSubtotalSelector).Text;
    }

    public string GetShippingAmount()
    {
        return Driver.GetElement(ShippingAmountSelector).Text;
    }

    public string GetOrderTotal()
    {
        return Driver.GetElement(OrderTotalSelector).Text;
    }

    public void PlaceOrder()
    {
        Driver.GetElement(PlaceOrderBtnSelector).Click();
    }
}