using System;
using System.Xml.Linq;
using Appointment.Pn.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Appointment.Pn.Controllers
{
    public class OutlookAddinController : Controller
    {

        private readonly IOutlookAddinService _outlookAddinService;
        
        public OutlookAddinController(IOutlookAddinService outlookAddinService)
        {
            _outlookAddinService = outlookAddinService;
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route("api/manifest.{format}"), FormatFilter]
        public XElement Manifest()
        {

            return _outlookAddinService.Manifest();
//            XElement xmlElement = new XElement("OfficeApp", 
//                new XAttribute("xmlns", 232),
//                new XElement("Id", "bla"));
//            return xmlElement;


        }
    }
}