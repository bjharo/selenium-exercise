using OpenQA.Selenium;
using Ui.Browser;

namespace Ui.PageObjects;

public class ProductListPage : Page
{
    private string ProductLinkSelectorBase =>
        "//a[contains(@class, 'product-item-link')][normalize-space(.) = '{0}']";

    public ProductListPage(IWebDriver driver) : base(driver)
    { }

    public bool IsLoaded(string productCategoryTitle)
    {
        return Driver.WaitForElementToHaveText(PageTitleSelector, productCategoryTitle);
    }

    public bool IsProductListed(string productName)
    {
        var selector = By.XPath(string.Format(ProductLinkSelectorBase, productName));
        return Driver.WaitForElementToBeDisplayed(selector);
    }

    public void ClickProduct(string productName)
    {
        var selector = By.XPath(string.Format(ProductLinkSelectorBase, productName));
        Driver.GetElement(selector).Click();
    }
}