using OpenQA.Selenium;
using Ui.Browser;

namespace Ui.PageObjects;

public class CustomerLoginPage : Page
{
    private By EmailFieldSelector => By.Id("email");
    private By PasswordFieldSelector => By.Id("pass");
    private By SignInBtnSelector => By.Id("send2");
    private By CreateAccountBtnSelector => By.CssSelector("a.create");
    
    public CustomerLoginPage(IWebDriver driver) : base(driver)
    { }
    
    public bool IsLoaded()
    {
        return Driver.WaitForElementToHaveText(PageTitleSelector, "Customer Login")
               && Driver.WaitForElementToBeEnabled(CreateAccountBtnSelector);
    }

    public CustomerLoginPage EnterEmail(string email)
    {
        Driver.GetElement(EmailFieldSelector).SendKeys(email);
        return this;
    }

    public CustomerLoginPage EnterPassword(string password)
    {
        Driver.GetElement(PasswordFieldSelector).SendKeys(password);
        return this;
    }

    public void SubmitLogin()
    {
        Driver.GetElement(SignInBtnSelector).Click();
    }
}