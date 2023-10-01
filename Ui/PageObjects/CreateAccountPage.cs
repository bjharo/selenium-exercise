using OpenQA.Selenium;
using Ui.Browser;

namespace Ui.PageObjects;

public class CreateAccountPage : Page
{
    private By FirstNameFieldSelector => By.Id("firstname");
    private By LastNameFieldSelector => By.Id("lastname");
    private By EmailFieldSelector => By.Id("email_address");
    private By PasswordFieldSelector => By.Id("password");
    private By ConfirmPasswordFieldSelector => By.Id("password-confirmation");
    private By CreateAccountBtnSelector => By.CssSelector("button.submit");
    private By FirstNameErrorSelector => By.Id("firstname-error");
    private By LastNameErrorSelector => By.Id("lastname-error");
    private By EmailErrorSelector => By.Id("email_address-error");
    private By PasswordErrorSelector => By.Id("password-error");
    private By PasswordConfirmationErrorSelector => By.Id("password-confirmation-error");
    
    public CreateAccountPage(IWebDriver driver) : base(driver)
    { }

    public bool IsLoaded()
    {
        return Driver.WaitForElementToHaveText(PageTitleSelector, "Create New Customer Account")
               && Driver.WaitForElementToBeEnabled(CreateAccountBtnSelector);
    }

    public CreateAccountPage EnterFirstName(string firstName)
    {
        Driver.GetElement(FirstNameFieldSelector).ClearAndSendKeys(firstName);
        return this;
    }

    public CreateAccountPage EnterLastName(string lastName)
    {
        Driver.GetElement(LastNameFieldSelector).ClearAndSendKeys(lastName);
        return this;
    }

    public CreateAccountPage EnterEmail(string email)
    {
        Driver.GetElement(EmailFieldSelector).ClearAndSendKeys(email);
        return this;
    }

    public CreateAccountPage EnterPassword(string password)
    {
        Driver.GetElement(PasswordFieldSelector).ClearAndSendKeys(password);
        return this;
    }

    public CreateAccountPage EnterConfirmPassword(string password)
    {
        Driver.GetElement(ConfirmPasswordFieldSelector).ClearAndSendKeys(password);
        return this;
    }

    public void SubmitNewAccount()
    {
        Driver.GetElement(CreateAccountBtnSelector).Click();
    }

    public string GetFirstNameError()
    {
        return Driver.GetElement(FirstNameErrorSelector).Text;
    }
    
    public string GetLastNameError()
    {
        return Driver.GetElement(LastNameErrorSelector).Text;
    }
    
    public string GetEmailError()
    {
        return Driver.GetElement(EmailErrorSelector).Text;
    }
    
    public string GetPasswordError()
    {
        return Driver.GetElement(PasswordErrorSelector).Text;
    }
    
    public string GetPasswordConfirmationError()
    {
        return Driver.GetElement(PasswordConfirmationErrorSelector).Text;
    }
}