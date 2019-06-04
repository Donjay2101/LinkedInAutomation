using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;

namespace LinkedInRequestSolution
{
    public class RequestSender
    {
        private static readonly Lazy<RequestSender> _instance = new Lazy<RequestSender>();
        private ChromeDriver _driver;


        public RequestSender()
        {

        }
        public static RequestSender Instance
        {
            get
            {
                return _instance.Value;
            }
        }


        public string Username { get; set; }
        public string Password { get; set; }



        private void ConnectToLinkedIn()
        {
            var drvier_location = this.GetType().Assembly.Location.Substring(0, this.GetType().Assembly.Location.LastIndexOf("\\"));
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            _driver = new ChromeDriver(drvier_location, options);
            _driver.Navigate().GoToUrl("https://www.linkedin.com/login");
            var email = _driver.FindElementById("username");
            var epassword = _driver.FindElementById("password");
            email.SendKeys(Username);
            epassword.SendKeys(Password);
            var submitButton = _driver.FindElement(By.XPath("//button[contains(@class,'btn__primary--large from__button--floating')]"));
            submitButton.Submit();

        }


        public void GotoNetworks()
        {
            _driver.Navigate().GoToUrl("https://www.linkedin.com/mynetwork/");
        }


        public void Init()
        {
            ConnectToLinkedIn();
        }

        public void SendRequest()
        {


            var overlayDiv = _driver.FindElementById("artdeco-modal-outlet");

            if (overlayDiv != null && !string.IsNullOrWhiteSpace(overlayDiv.GetAttribute("innerHTML")))
            {
                Console.WriteLine("You are out of inivitation today. Run script again tommorrow");
                return;
            }
            else
            {
                try
                {
                    ScrollTo(_driver, 0, 600);
                    var ul_Element = _driver.FindElement(By.XPath("//ul[contains(@class, 'js-discover-entity-list__pymk discover-entity-list') and contains(@class, 'ember-view')]"));
                    var li_elements = ul_Element.FindElements(By.TagName("li"));
                    if (li_elements.Count == 0)
                    {
                        ScrollTo(_driver, 0, -1000);
                    }
                    foreach (var elem in li_elements)
                    {
                        var data = elem.GetAttribute("innerHTML");
                        var footer = elem.FindElement(By.TagName("footer"));
                        var btn = footer.FindElement(By.TagName("button"));
                        btn.Click();
                    }

                }
                catch (StaleElementReferenceException)
                {

                }
                SendRequest();
            }
        }



        public void SearchInWeb(string serachlabel,string location)
        {
            GotoNetworks();
            var search_div = _driver.FindElementById("global-nav-typeahead");
            var search_box = search_div.FindElement(By.TagName("input"));
            search_box.Clear();
            search_box.SendKeys(serachlabel);
            search_box.SendKeys(Keys.Enter);
            Thread.Sleep(4000);
            //var ul = _driver.FindElement(By.XPath("//ul[Contians(@class,'search-results__list') and contains(@class,'list-style-none')]"));
            LocationFilter(location);
            SendRequest2();
        }


        private IWebElement FindDivElement()
        {
            return _driver.FindElement(By.XPath("//div[contains(@class,'search-basic-typeahead') and contains(@class,'search-vertical-typeahead') and contains(@class,'ember-view')]"));
        }
        private void LocationFilter(string location)
        {
            try
            {
                var locationUl = _driver.FindElement(By.XPath("//ul[contains(@class,'peek-carousel__slides') and contains(@class,'js-list')]"));
                var li = locationUl.FindElement(By.XPath("//li[contains(@class,'search-s-facet--geoRegion')]"));
                li.Click();

                var locationdiv = FindDivElement();
                var inputText = locationdiv.FindElement(By.TagName("input"));
                inputText.Clear();
                inputText.SendKeys(location);
                locationdiv = FindDivElement();
                var locDropdown_list = locationdiv.FindElements(By.XPath("//div[contains(@class,'basic-typeahead__triggered-content') and contains(@class,'search-s-add-facet__typeahead-tray')]"));
                if (locDropdown_list.Count > 0)
                {
                    var divs = locDropdown_list[0].FindElements(By.TagName("div"));
                    if(divs.Count>0)
                    {
                        divs[2].Click();
                    }
                   // Debug.Write(locDropdown_list[0] .GetAttribute("innerHTML"));
                }
                //var classNames = locationdiv.GetAttribute("class").Split(" ");
                //if (classNames != null && classNames.GetValue(classNames.Length - 1).ToString() == "is-active")
                //{
                //    Thread.Sleep(1000);
                //    var suggestion_list = locationdiv.FindElement(By.TagName("ul"))
                //        .FindElement(By.TagName("li"));
                //    suggestion_list.Click();
                //}
                var liist = _driver.FindElements(By.XPath("//fieldset[contains(@class,'search-s-facet__values')]"));
                if (liist.Count > 0)
                {
                    var btn = liist[1].FindElements(By.TagName("button"))[1];
                    btn.Click();
                    Thread.Sleep(4000);
                }
            }
            catch
            {
                throw;
            }

        }



        private void SendRequest2()
        {
            try
            {
                if (!_driver.FindElement(By.XPath("//ul[contains(@class,'search-results__list')]")).Displayed)
                {
                    return;
                }
                else
                {
                    var searchedResults = _driver.FindElement(By.XPath("//ul[contains(@class,'search-results__list')]")).FindElements(By.TagName("li"));
                    int count = 1;
                    int y = 500;
                    foreach (var item in searchedResults)
                    {
                        if (count == 6)
                        {
                            ScrollTo(_driver, 0, y);
                            count = 1;
                        }
                        try
                        {
                            //if (item.FindElement(By.TagName("button")).Displayed)
                            //{
                                ClickButton(item);
                            //}
                        }
                        catch (StaleElementReferenceException)
                        {
                            ClickButton(item);
                        }

                        count++;
                    }
                    y += 500;
                    ScrollTo(_driver, 0, y);

                    try
                    {
                        Thread.Sleep(3000);
                        ClickNext(y);
                    }
                    catch (Exception)
                    {
                        ClickNext(y);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex);

            }
            SendRequest2();
        }

        private void ClickNext(int y=1000)
        {
            
            var btnNext = _driver.FindElementByClassName("artdeco-pagination__button--next");
            if (btnNext != null)
            {
                btnNext.Click();
                ScrollTo(_driver, 0, -y);
            }
            Thread.Sleep(5000);
        }


        private void ClickButton(IWebElement item)
        {
            var divs = item.FindElements(By.ClassName("search-result__actions"));
            if(divs.Count <= 0)
            {
                return;
            }
            var div = divs[0];
            var innerHTML = div.GetAttribute("innerHTML");
            var index = innerHTML.IndexOf("premium-upsell-link ember-view");
            if (index <= 0)
            {
                var list = div.FindElements(By.TagName("button"));
                if(list.Count > 0)
                {
                    var connectBtn = list[0];
                    var innerText = connectBtn.GetAttribute("innerHTML").Replace("\r", "").Replace("\n", "").Trim();
                    //var classValues = connectBtn.GetCssValue("class");
                    // var classindex = classValues.IndexOf("message-anywhere-button");
                    if (innerText != "Invite Sent" && innerText != "Message" && innerText != "Following" && innerText != "Follow")
                    {
                        connectBtn.Click();
                        var addDiv = _driver.FindElement(By.XPath("//div[contains(@class,'modal-wormhole-content')]"));
                        if (!string.IsNullOrWhiteSpace(addDiv.GetAttribute("innerHTML")))
                        {
                            var btnsednNow = addDiv.FindElements(By.TagName("button"))[2];
                            btnsednNow.Click();
                            //_driver.Navigate().Refresh();
                            Thread.Sleep(2000);
                        }
                        else
                        {

                        }
                    }
                }
            }
            
        }
        public void ScrollTo(ChromeDriver _driver, int xPosition = 0, int yPosition = 0)
        {
            var js = String.Format("window.scrollTo({0}, {1})", xPosition, yPosition);
            IJavaScriptExecutor jsEx = (IJavaScriptExecutor)_driver;
            jsEx.ExecuteScript(js);
        }
    }
}



///
//var toast = _driver.FindElementById("artdeco-toasts");
//if (toast != null)
//{
//    try
//    {
//        var ul = toast.FindElement(By.XPath("//ul[contains(@class, 'artdeco-toasts_toasts')]"));


//        var li = ul.FindElement(By.TagName("li"));
//        if (li != null)
//        {
//            var btnClose = li.FindElement(By.TagName("button"));
//            if (btnClose != null)
//            {
//                btnClose.Click();
//            }
//            Thread.Sleep(2000);
//        }
//    }
//    catch (NoSuchElementException)
//    {

//    }
//}
//Thread.Sleep(2000);