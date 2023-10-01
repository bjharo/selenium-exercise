using OpenQA.Selenium;
using Ui.Browser;

namespace Ui.PageObjects;

public class PageHeaderWidget : Widget
{
    private By ParentSelector => By.CssSelector("header.page-header");
    private By SignInLinkSelector => By.CssSelector(".page-header .authorization-link > a[href *= 'account/login/']");
    private By CreateAccountLinkSelector => By.CssSelector(".page-header a[href *= 'account/create']");
    private By CartItemCountSelector => By.CssSelector(".minicart-wrapper .counter-number");
    private By CartLinkSelector => By.CssSelector(".minicart-wrapper > a.showcart");
    private By WelcomeMsgSelector => By.CssSelector(".greet.welcome");
    private By CartLoadingSelector => By.CssSelector(".qty._block-content-loading");
    
    public PageHeaderWidget(IWebDriver driver) : base(driver) { }
    
    public bool IsLoaded()
    {
        return Driver.WaitForElementToBeDisplayed(ParentSelector) &&
               Driver.WaitForElementToContainText(WelcomeMsgSelector, "welcome");
    }

    public void WaitForCartQuantityToUpdate()
    {
        Driver.WaitForElementToNotBeDisplayed(CartLoadingSelector);
    }

    public void ClickCreateAccount()
    {
        Driver.GetElement(CreateAccountLinkSelector).Click();
    }

    public string GetWelcomeMessage()
    {
        return Driver.GetElement(WelcomeMsgSelector).Text;
    }

    public bool WaitForWelcomeMessageToContain(string expectedText)
    {
        return Driver.WaitForElementToHaveText(WelcomeMsgSelector, expectedText);
    }

    public int GetCurrentCartQuantity()
    {
        var quantityText = Driver.WaitForElementToBeDisplayed(CartItemCountSelector, TimeSpan.FromSeconds(2))
            ? Driver.GetElement(CartItemCountSelector).Text
            : "0";
        
        
        if (int.TryParse(quantityText, out var quantity))
        {
            return quantity;
        }

        throw new Exception($"The cart quantity is not a valid number. {quantityText}");
    }
}