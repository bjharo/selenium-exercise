using System.Text.RegularExpressions;
using OpenQA.Selenium;
using Ui.Browser;
using Ui.Enums;

namespace Ui.PageObjects;

public class ProductPage : Page
{
    private By AddToCardBtnSelector => By.Id("product-addtocart-button");
    private By SuccessMessageSelector => By.CssSelector("[data-ui-id = 'message-success']");
    private By ShoppingCartSuccessMessageLinkSelector => By.CssSelector("[data-ui-id = 'message-success'] a");
    private By QuantityFieldSelector => By.Id("qty");
    private By LoaderSelector => By.Id("sk-loader");
    private By PriceSelector => By.CssSelector(".product-info-price .price");

    public ProductPage(IWebDriver driver) : base(driver)
    { }

    public bool IsLoaded(string productName)
    {
        return Driver.WaitForElementToHaveText(PageTitleSelector, productName)
               && Driver.WaitForElementToBeEnabled(AddToCardBtnSelector);
    }

    public decimal GetPrice()
    {
        var priceText = Driver.GetElement(PriceSelector).Text;
        var priceMatch = Regex.Match(priceText, @"\d+\.\d{2}");

        if (priceMatch.Success)
        {
            return Decimal.Parse(priceMatch.Value);
        }

        throw new Exception(
            $"Could not confirm that the price displayed is a valid decimal value. The displayed price is {priceText}");
    }

    public ProductPage AddToCart()
    {
        Driver.GetElement(AddToCardBtnSelector).Click();
        return this;
    }

    public ProductPage SelectSize(ProductSize size)
    {
        SelectSwatch("size", size.GetDescription());
        return this;
    }
    
    public ProductPage SelectColor(ProductColor color)
    {
        SelectSwatch("color", color.ToString());
        return this;
    }

    public ProductPage EnterQty(int quantity)
    {
        Driver.GetElement(QuantityFieldSelector).ClearExistingValueAndEnter(quantity.ToString());
        return this;
    }

    public string GetSuccessMessage()
    {
        return Driver.GetElement(SuccessMessageSelector).Text;
    }

    public void GoToCartFromSuccessMessage()
    {
        Driver.GetElement(ShoppingCartSuccessMessageLinkSelector).Click();
    }
    
    private void SelectSwatch(string swatchAttributeName, string swatchOption)
    {
        var selector = By.CssSelector($".swatch-attribute.{swatchAttributeName} .swatch-option[aria-label = '{swatchOption}']");
        Driver.GetElement(selector).Click();
    }
}