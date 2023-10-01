using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Bogus;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Shouldly;
using Tests.DataObjects;
using Ui.Browser;
using Ui.Enums;
using Ui.PageObjects;
using Address = Tests.DataObjects.Address;

namespace Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class Tests
{
    private static string AppUrl => "https://magento.softwaretestingboard.com/";
    private BrowserFactory BrowserFactory { get; set; } = null!;
    private IWebDriver Driver => BrowserFactory.CurrentBrowser;
    private Faker Faker { get; } = new("en_US");
    private ExtentReports ExReport { get; } = new();
    private ThreadLocal<ExtentTest> ExTest { get; } = new();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var headlessEnv = Environment.GetEnvironmentVariable("AUTOMATION_HEADLESS");
        var useHeadless = !string.IsNullOrEmpty(headlessEnv) 
                          && (bool.TryParse(headlessEnv, out var parsedHeadless) && parsedHeadless);

        var browserEnv = Environment.GetEnvironmentVariable("AUTOMATION_BROWSER");
        var browser = string.IsNullOrEmpty(browserEnv)
            ? BrowserType.Chrome
            : Enum.TryParse<BrowserType>(browserEnv, out BrowserType parsedType)
                ? parsedType
                : BrowserType.Chrome;

        BrowserFactory = new BrowserFactory(browser, useHeadless);

        ExReport.AttachReporter(new ExtentSparkReporter("TestReport.html"));
        ExReport.AddSystemInfo("Browser", browser.ToString());
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        ExReport.Flush();
    }
    
    [SetUp]
    public void Setup()
    {
        ExTest.Value = ExReport.CreateTest(TestContext.CurrentContext.Test.Name);
        BrowserFactory.CreateBrowser();
    }
    
    [TearDown]
    public void TearDown()
    {
        BrowserFactory.DisposeCurrentBrowser();
        
        var stackTrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
            ? string.Empty
            : TestContext.CurrentContext.Result.StackTrace;

        switch (TestContext.CurrentContext.Result.Outcome.Status)
        {
            case TestStatus.Failed:
                ExTest.Value!.Log(Status.Fail, "Fail");
                ExTest.Value!.Log(Status.Fail, TestContext.CurrentContext.Result.Message);
                ExTest.Value!.Log(Status.Fail, TestContext.CurrentContext.Result.StackTrace);
                break;
            case TestStatus.Passed:
                ExTest.Value!.Log(Status.Pass, "Pass");
                break;
            case TestStatus.Skipped:
                ExTest.Value!.Log(Status.Skip, "Skipped");
                break;
            default:
                ExTest.Value!.Log(Status.Warning, "Warning");
                break;
        }

        ExReport.Flush();
    }

    [Test]
    [TestCaseSource(nameof(CreateAccountPurchaseOneProductTestCases))]
    public void CreateAccountPurchaseOneProduct(string productCategory, string[] productCategoryNavigation, string productName, ProductSize size, ProductColor color, int productQty, ShippingType shipType)
    {
        var account = new Account(Faker.Name.FirstName(), Faker.Name.LastName(), Faker.Internet.Email(),
            Faker.Internet.Password(10, prefix: "Ab@1"));
        var shippingAddress = new Address(Faker.Address.StreetAddress(), null, null, Faker.Address.City(),
            Faker.Address.State(), Faker.Address.ZipCode("#####"), "United States", null);
        var formattedAddress = $"{shippingAddress.StreetOne}{Environment.NewLine}{shippingAddress.City}, " +
                               $"{shippingAddress.State} {shippingAddress.PostalCode}{Environment.NewLine}" +
                               $"{shippingAddress.Country}";
        var phoneNumber = Faker.Phone.PhoneNumber("###########");

        var shippingPage = new ShippingPage(Driver);
        var paymentPage = new PaymentMethodPage(Driver);
        var cartPage = new CartPage(Driver);
        var productPage = new ProductPage(Driver);
        
        NavigateToStore();
        CreateAccountFromHeader(account);
        NavigateToProductListPage(productCategory, productCategoryNavigation);
        SelectProductFromListPage(productName);
        SelectClothingProductOptions(size, color, productQty);

        var productPrice = productPage.GetPrice();
        new ProductPage(Driver).GoToCartFromSuccessMessage();
        cartPage.IsLoaded().ShouldBeTrue("Could not confirm the cart was loaded.");
        
        VerifyProductInShoppingCart(productName, productPrice, productQty, size, color);

        cartPage.ProceedToCheckout();
        shippingPage.IsLoaded().ShouldBeTrue("Could not confirm the shipping page loaded.");

        VerifyNameAutoFilledOnShippingPage(account.FirstName, account.LastName);
        EnterShippingInfo(shippingAddress, phoneNumber, shipType);

        var shippingCost = shippingPage.GetShippingPrice(shipType);
        shippingPage.ClickNext();
        paymentPage.IsLoaded().ShouldBeTrue("Could not confirm the payment method page loaded.");

        VerifyBillingAddressOnPaymentPage(account.FirstName, account.LastName, formattedAddress, phoneNumber);
        VerifyOrderAmountsOnPaymentPage(productPrice * productQty, shippingCost);
        VerifyShippingInformation(account.FirstName, account.LastName, formattedAddress, phoneNumber, shipType);

        paymentPage.PlaceOrder();
        new ThankYouPage(Driver).IsLoaded()
            .ShouldBeTrue("Could not confirm the thank you page was loaded after placing an order.");

        VerifyOrderNumberIsDisplayed();
    }

    [Test]
    [TestCaseSource(nameof(VerifyCreateAccountRequiredFieldErrorsTestCases))]
    public void VerifyCreateAccountRequiredFieldErrors(Action<CreateAccountPage> clearField,
        Func<CreateAccountPage, string> getFieldError, string fieldName)
    {
        var account = new Account(Faker.Name.FirstName(), Faker.Name.LastName(), Faker.Internet.Email(),
            Faker.Internet.Password(10));
        
        var pageHeader = new PageHeaderWidget(Driver);
        var createAccountPage = new CreateAccountPage(Driver);

        NavigateToStore();
        
        pageHeader.ClickCreateAccount();

        createAccountPage.IsLoaded()
            .ShouldBeTrue(
                "Could not confirm the create account page was loaded after click create account in the header.");

        createAccountPage.EnterFirstName(account.FirstName)
            .EnterLastName(account.LastName)
            .EnterEmail(account.EmailAddress)
            .EnterPassword(account.Password)
            .EnterConfirmPassword(account.Password);

        clearField(createAccountPage);
        createAccountPage.SubmitNewAccount();
        getFieldError(createAccountPage).ShouldBe("This is a required field.",
            $"The {fieldName} required field error was not correct.");
    }
    
    private void NavigateToStore()
    {
        Driver.Navigate().GoToUrl(AppUrl);

        var pageHeader = new PageHeaderWidget(Driver);
        var sectionsNav = new SectionsNavWidget(Driver);

        pageHeader.IsLoaded()
            .ShouldBeTrue(
                "Could not confirm that the page header was loaded after navigating to the store's homepage.");
        sectionsNav.IsLoaded()
            .ShouldBeTrue(
                "Could not confirm that the section navigation bar was loaded after navigating to the store's homepage.");
    }

    private void CreateAccountFromHeader(Account account)
    {
        var pageHeader = new PageHeaderWidget(Driver);
        var createAccountPage = new CreateAccountPage(Driver);
        var myAccountPage = new MyAccountPage(Driver);

        pageHeader.ClickCreateAccount();

        createAccountPage.IsLoaded()
            .ShouldBeTrue(
                "Could not confirm the create account page was loaded after click create account in the header.");

        createAccountPage.EnterFirstName(account.FirstName)
            .EnterLastName(account.LastName)
            .EnterEmail(account.EmailAddress)
            .EnterPassword(account.Password)
            .EnterConfirmPassword(account.Password)
            .SubmitNewAccount();

        myAccountPage.IsLoaded()
            .ShouldBeTrue("Could not confirm that the my account page was loaded after submitting a new account form.");

        var nameAndEmail = myAccountPage.GetNameAndEmail();

        nameAndEmail.ShouldSatisfyAllConditions(
            () => nameAndEmail.ShouldContain(account.FirstName),
            () => nameAndEmail.ShouldContain(account.LastName),
            () => nameAndEmail.ShouldContain(account.EmailAddress));

        pageHeader.WaitForWelcomeMessageToContain($"Welcome, {account.FirstName} {account.LastName}!").ShouldBeTrue(
            $"Could not confirm that the welcome message was updated after creating a new account. " +
            $"The current message is {pageHeader.GetWelcomeMessage()}");
    }

    private void NavigateToProductListPage(string productCategory, params string[] sections)
    {
        var navBar = new SectionsNavWidget(Driver);
        var productListPage = new ProductListPage(Driver);
        
        navBar.NavigateToSection(sections);

        productListPage.IsLoaded(productCategory)
            .ShouldBeTrue($"Could not confirm that the {productCategory} list page was loaded.");
    }

    private void SelectProductFromListPage(string productName)
    {
        var productListPage = new ProductListPage(Driver);
        var productPage = new ProductPage(Driver);

        productListPage.IsProductListed(productName).ShouldBeTrue($"Could not find the product named {productName}.");
        productListPage.ClickProduct(productName);

        productPage.IsLoaded(productName)
            .ShouldBeTrue(
                $"Could not confirm that the page for {productName} was loaded after clicking the product " +
                $"on the list page.");
    }

    private void SelectClothingProductOptions(ProductSize size, ProductColor color, int quantity)
    {
        var productPage = new ProductPage(Driver);
        var pageHeader = new PageHeaderWidget(Driver);
        var currentCartQuantity = pageHeader.GetCurrentCartQuantity();
        var productName = productPage.GetPageTitle();

        productPage.SelectSize(size)
            .SelectColor(color)
            .EnterQty(quantity)
            .AddToCart();

        pageHeader.WaitForCartQuantityToUpdate();
        pageHeader.GetCurrentCartQuantity().ShouldBe(currentCartQuantity + quantity);
        productPage.GetSuccessMessage().ShouldBe($"You added {productName} to your shopping cart.");
    }

    private void VerifyProductInShoppingCart(string productName, decimal price, int quantity, ProductSize size,
        ProductColor color)
    {
        var cartPage = new CartPage(Driver);

        var cartItem = cartPage.GetCartItem(productName);

        cartItem.productOnPage.ShouldBeTrue($"Could not confirm that the {productName} was listed in the cart.");
        cartItem.itemWidget.ShouldNotBeNull().ShouldSatisfyAllConditions(
            itemWidget => itemWidget.GetPrice().ShouldBe(price.ToString("C")),
            itemWidget => itemWidget.GetQuantity().ShouldBe(quantity.ToString()),
            itemWidget => itemWidget.GetSubtotal().ShouldBe(Math.Round(price * quantity, 2).ToString("C")),
            itemWidget => itemWidget.GetOptions().ShouldBe($"Size {size.GetDescription()} Color {color.ToString()}"));
    }

    private void VerifyNameAutoFilledOnShippingPage(string firstName, string lastName)
    {
        var shippingPage = new ShippingPage(Driver);
        shippingPage.ShouldSatisfyAllConditions(
            () => shippingPage.WaitForFirstNameText(firstName).ShouldBeTrue(),
            () => shippingPage.WaitForLastNameText(lastName).ShouldBeTrue());
    }

    private void EnterShippingInfo(Address address, string phoneNumber, ShippingType shipType, string? firstName = null,
        string? lastName = null)
    {
        var shippingPage = new ShippingPage(Driver);

        if (firstName is not null)
        {
            shippingPage.EnterFirstName(firstName);
        }

        if (lastName is not null)
        {
            shippingPage.EnterLastName(lastName);
        }

        shippingPage.EnterStreetAddressOne(address.StreetOne!)
            .EnterCity(address.City!)
            .SelectCountry(address.Country!)
            .SelectState(address.State!)
            .EnterPostalCode(address.PostalCode!)
            .EnterPhone(phoneNumber)
            .SelectShipping(shipType);
    }

    private void VerifyBillingAddressOnPaymentPage(string firstName, string lastName, string address,
        string phoneNumber)
    {
        var billingAddressDetails = new PaymentMethodPage(Driver).GetBillingAddressDetails();

        billingAddressDetails.ShouldSatisfyAllConditions(
            () => billingAddressDetails.ShouldContain($"{firstName} {lastName}"),
            () => billingAddressDetails.ShouldContain(address),
            () => billingAddressDetails.ShouldContain(phoneNumber));
    }
    
    private void VerifyShippingInformation(string firstName, string lastName, string address,
        string phoneNumber, ShippingType shipType)
    {
        var paymentPage = new PaymentMethodPage(Driver);

        paymentPage.GetShipToDetails().ShouldSatisfyAllConditions(
            details => details.ShouldContain($"{firstName} {lastName}"),
            details => details.ShouldContain(address), 
            details => details.ShouldContain(phoneNumber));

        var shipTypeName = shipType switch
        {
            ShippingType.FixedFlatRate => "Flat Rate - Fixed",
            ShippingType.TableRateBestWay => "Best Way - Table Rate",
            _ => throw new ArgumentException("Unknown shipping type.")
        };

        paymentPage.GetShipViaDetails().ShouldBe(shipTypeName);
    }

    private void VerifyOrderAmountsOnPaymentPage(decimal subtotal, decimal shipping)
    {
        var paymentPage = new PaymentMethodPage(Driver);

        paymentPage.ShouldSatisfyAllConditions(
            () => paymentPage.GetSubTotal().ShouldBe(subtotal.ToString("C")),
            () => paymentPage.GetShippingAmount().ShouldBe(shipping.ToString("C")),
            () => paymentPage.GetOrderTotal().ShouldBe((subtotal + shipping).ToString("C")));
    }

    private void VerifyOrderNumberIsDisplayed()
    {
        var orderDetails = new ThankYouPage(Driver).GetCheckoutSuccessMessage();
        orderDetails.ShouldMatch(@"Your order number is: \d{9}");
    }
    
    private static IEnumerable<TestCaseData> CreateAccountPurchaseOneProductTestCases()
    {
        return new[]
        {
            new TestCaseData("Jackets", new[] { "Women", "Tops", "Jackets" }, "Juno Jacket", ProductSize.M,
                ProductColor.Blue, 2, ShippingType.FixedFlatRate).SetName("{m}_WomensJacket"),
            new TestCaseData("Pants", new[] { "Men", "Bottoms", "Pants" }, "Geo Insulated Jogging Pant",
                ProductSize.ThirtyFour, ProductColor.Red, 3, ShippingType.TableRateBestWay).SetName("{m}_MensPant")
        };
    }
    
    private static IEnumerable<TestCaseData> VerifyCreateAccountRequiredFieldErrorsTestCases()
    {
        return new[]
        {
            new TestCaseData(new Action<CreateAccountPage>(page => page.EnterFirstName("")),
                    new Func<CreateAccountPage, string>(page => page.GetFirstNameError()), "First Name")
                .SetName("{m}_FirstName"),
            new TestCaseData(new Action<CreateAccountPage>(page => page.EnterLastName("")),
                    new Func<CreateAccountPage, string>(page => page.GetLastNameError()), "Last Name")
                .SetName("{m}_LastName"),
            new TestCaseData(new Action<CreateAccountPage>(page => page.EnterEmail("")),
                new Func<CreateAccountPage, string>(page => page.GetEmailError()), "Email").SetName("{m}_Email"),
            new TestCaseData(new Action<CreateAccountPage>(page => page.EnterPassword("")),
                    new Func<CreateAccountPage, string>(page => page.GetPasswordError()), "Password")
                .SetName("{m}_Password"),
            new TestCaseData(new Action<CreateAccountPage>(page => page.EnterConfirmPassword("")),
                new Func<CreateAccountPage, string>(page => page.GetPasswordConfirmationError()),
                "Password Confirmation").SetName("{m}_PasswordConfirmation")
        };
    }
}