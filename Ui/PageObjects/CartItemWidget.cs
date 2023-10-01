using OpenQA.Selenium;
using Ui.Browser;

namespace Ui.PageObjects;

public class CartItemWidget : Widget
{
    private IWebElement Parent { get; init; }
    private By ProductItemLinkSelector => By.CssSelector(".product-item-name > a");
    private By QtyFieldSelector => By.CssSelector("input.qty");
    private By PriceSelector => By.CssSelector("td.price span.price");
    private By SubtotalSelector => By.CssSelector("td.subtotal span.price");
    private By ItemOptionsSelector => By.CssSelector(".item-options");

    public CartItemWidget(IWebDriver driver, IWebElement parent) : base(driver)
    {
        Parent = parent;
    }

    public string GetProductName()
    {
        return Parent.GetElement(ProductItemLinkSelector).Text;
    }

    public string GetQuantity()
    {
        return Parent.GetElement(QtyFieldSelector).GetAttribute("value");
    }

    public string GetPrice()
    {
        return Parent.GetElement(PriceSelector).Text;
    }

    public string GetSubtotal()
    {
        return Parent.GetElement(SubtotalSelector).Text;
    }

    public string GetOptions()
    {
        return Parent.GetElement(ItemOptionsSelector).Text.Replace(Environment.NewLine, " ");
    }
}