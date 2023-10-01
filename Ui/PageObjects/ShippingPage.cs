using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Ui.Browser;
using Ui.Enums;

namespace Ui.PageObjects;

public class ShippingPage : Page
{
    private By StepTitleSelector => By.CssSelector(".step-title");
    private By FirstNameFieldSelector => By.CssSelector("input[name = 'firstname']");
    private By LastNameFieldSelector => By.CssSelector("input[name = 'lastname']");
    private By CompanyFieldSelector => By.CssSelector("input[name = 'company']");
    private By StreetOneFieldSelector => By.CssSelector("input[name = 'street[0]']");
    private By StreetTwoFieldSelector => By.CssSelector("input[name = 'street[1]']");
    private By StreetThreeFieldSelector => By.CssSelector("input[name = 'street[2]']");
    private By CityFieldSelector => By.CssSelector("input[name = 'city']");
    private By StateDropdownSelector => By.CssSelector("select[name = 'region_id']");
    private By PostalCodeFieldSelector => By.CssSelector("input[name = 'postcode']");
    private By CountryDropdownSelector => By.CssSelector("select[name = 'country_id']");
    private By PhoneFieldSelector => By.CssSelector("input[name = 'telephone']");
    private By NextBtnSelector => By.CssSelector("button.continue.primary");
    private By FlatRateRadBtnSelector => By.CssSelector("input.radio[value = 'flatrate_flatrate']");
    private By BestWayRadBtnSelector => By.CssSelector("input.radio[value = 'tablerate_bestway']");
    
    public ShippingPage(IWebDriver driver) : base(driver)
    { }

    public bool IsLoaded()
    {
        return Driver.WaitForElementToHaveText(StepTitleSelector, "Shipping Address")
               && Driver.WaitForElementToBeEnabled(NextBtnSelector)
               && Driver.WaitForElementToNotBeDisplayed(By.CssSelector(".loading-mask"));
    }

    public string GetFirstName()
    {
        return Driver.GetElement(FirstNameFieldSelector).Text;
    }

    public string GetLastName()
    {
        return Driver.GetElement(LastNameFieldSelector).Text;
    }

    public ShippingPage EnterFirstName(string firstName)
    {
        Driver.GetElement(FirstNameFieldSelector).SendKeys(firstName);
        return this;
    }

    public ShippingPage EnterLastName(string lastName)
    {
        Driver.GetElement(LastNameFieldSelector).SendKeys(lastName);
        return this;
    }

    public ShippingPage EnterStreetAddressOne(string streetAddress)
    {
        Driver.GetElement(StreetOneFieldSelector).SendKeys(streetAddress);
        return this;
    }
    
    public ShippingPage EnterCity(string city)
    {
        Driver.GetElement(CityFieldSelector).SendKeys(city);
        return this;
    }
    
    public ShippingPage EnterPostalCode(string postalCode)
    {
        Driver.GetElement(PostalCodeFieldSelector).SendKeys(postalCode);
        return this;
    }

    public ShippingPage SelectCountry(string country)
    {
        var select = new SelectElement(Driver.GetElement(CountryDropdownSelector));
        select.SelectByText(country);
        return this;
    }

    public ShippingPage SelectState(string state)
    {
        var select = new SelectElement(Driver.GetElement(StateDropdownSelector));
        select.SelectByText(state);
        return this;
    }

    public ShippingPage EnterPhone(string phoneNumber)
    {
        Driver.GetElement(PhoneFieldSelector).SendKeys(phoneNumber);
        return this;
    }

    public ShippingPage SelectShipping(ShippingType shipType)
    {
        switch (shipType)
        {
            case ShippingType.FixedFlatRate:
                Driver.GetElement(FlatRateRadBtnSelector).Click();
                break;
            case ShippingType.TableRateBestWay:
                Driver.GetElement(BestWayRadBtnSelector).Click();
                break;
            default:
                throw new ArgumentException($"Unable to select this ship type, {shipType.ToString()}");
        }

        return this;
    }

    public decimal GetShippingPrice(ShippingType shipType)
    {
        var selector = shipType switch
        {
            ShippingType.FixedFlatRate => By.XPath("//td[preceding-sibling::td/input[@value = 'flatrate_flatrate']]/span/span[@class = 'price']"),
            ShippingType.TableRateBestWay => By.XPath("//td[preceding-sibling::td/input[@value = 'tablerate_bestway']]/span/span[@class = 'price']"),
            _ => throw new ArgumentException($"{shipType.ToString()} is an unknown ship type.")
        };

        var shipPriceText = Driver.GetElement(selector).Text;
        var priceMatch = Regex.Match(shipPriceText, @"\d+\.\d{2}");
        
        if (priceMatch.Success)
        {
            return Decimal.Parse(priceMatch.Value);
        }

        throw new Exception(
            $"Could not confirm that the shipping cost displayed is a valid decimal value. The displayed price is {shipPriceText}");
    }

    public void ClickNext()
    {
        Driver.GetElement(NextBtnSelector).Click();
    }

    public bool WaitForFirstNameText(string name)
    {
        return Driver.WaitForElementToHaveValue(FirstNameFieldSelector, name);
    }

    public bool WaitForLastNameText(string name)
    {
        return Driver.WaitForElementToHaveValue(LastNameFieldSelector, name);
    }
}