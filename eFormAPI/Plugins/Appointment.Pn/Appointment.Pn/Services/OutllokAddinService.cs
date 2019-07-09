using System;
using System.Linq;
using System.Xml.Linq;
using Appointment.Pn.Abstractions;
using eFormShared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.AppointmentBase.Infrastructure.Data;
using Microting.eFormApi.BasePn.Abstractions;

namespace Appointment.Pn.Services
{
    public class OutllokAddinService :IOutlookAddinService
    {
        private readonly ILogger<AppointmentPnSettingsService> _logger;
        private readonly IAppointmentLocalizationService _trashInspectionLocalizationService;
        private readonly AppointmentPnDbContext _dbContext;
        private readonly IEFormCoreService _coreHelper;
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OutllokAddinService(ILogger<AppointmentPnSettingsService> logger,
            IAppointmentLocalizationService trashInspectionLocalizationService,
            AppointmentPnDbContext dbContext,
            IEFormCoreService coreHelper,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dbContext = dbContext;
            _coreHelper = coreHelper;
            _httpContextAccessor = httpContextAccessor;
            _trashInspectionLocalizationService = trashInspectionLocalizationService;
            
        }
        
        public XElement Manifest()
        {
            XNamespace xmlns = XNamespace.Get("http://schemas.microsoft.com/office/appforoffice/1.1");
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            XNamespace bt = XNamespace.Get("http://schemas.microsoft.com/office/officeappbasictypes/1.0");
            XNamespace mailappor = XNamespace.Get("http://schemas.microsoft.com/office/mailappversionoverrides/1.0");
            XNamespace type = XNamespace.Get("MailApp");
            var pluginConfiguration = _dbContext.PluginConfigurationValues.SingleOrDefault(x => x.Name == "AppointmentBaseSettings:OutlookAddinId");


            XElement xDocument = new XElement("OfficeApp",
//                new XAttribute("xmlns", "http://schemas.microsoft.com/office/appforoffice/1.1"),
                new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XAttribute(XNamespace.Xmlns + "bt", bt),
                new XAttribute(XNamespace.Xmlns + "mailappor", mailappor),
                new XAttribute(XNamespace.Xmlns + "type", type),
                new XElement("Id", pluginConfiguration.Value),
                new XElement("Version", "1.0.0.0"), // PluginAssembly().GetName().Version.ToString()
                new XElement("ProviderName", "Microting"),
                new XElement("DefaultLocale", "en-US"),
                    new XElement("IconUrl", new XAttribute("DefaultValue", $"{_coreHelper.GetCore().GetSdkSetting(Settings.httpServerAddress)}/assets/icon-32.png")),
                    new XElement("HighResolutionIconUrl", new XAttribute("DefaultValue", $"{_coreHelper.GetCore().GetSdkSetting(Settings.httpServerAddress)}/assets/icon-144.png")),
                    new XElement("SupportUrl", new XAttribute("DefaultValue", $"{_coreHelper.GetCore().GetSdkSetting(Settings.httpServerAddress)}")),
                new XElement("Hosts",
                    new XElement("Host",
                        new XAttribute("Name", "Mailbox"))),
                new XElement("Requirements",
                    new XElement("Sets",
                        new XElement("Set",
                            new XAttribute("Name", "Mailbox"),
                            new XAttribute("MinVersion", "1.1")))),
                new XElement("FormSettings",
                    new XElement("Form",
                        new XAttribute(xsi+"type", "ItemRead"),
                        new XElement("DesktopSettings",
                            new XElement("SourceLocation",
                                new XAttribute("DefaultValue", $"{_coreHelper.GetCore().GetSdkSetting(Settings.httpServerAddress)}")
                            ),
                            new XElement("RequestHeight", 250)
                        )
                    )
                ),
                new XElement("Permissions", "ReadWriteMailbox"),
                new XElement("Rule",
                    new XAttribute(xsi+"type", "RuleCollection"),
                    new XAttribute("Mode", "Or"),
                    new XElement("Rule",
                        new XAttribute(xsi+"type", "ItemIs"),
                        new XAttribute("ItemType", "Appointment"),
                        new XAttribute("FromType", "Edit")
                    )
                ),
                new XElement("DisableEntityHighlighting", false),
                new XElement("VersionOverrides",
//                        new XAttribute("xmlns", @"http://schemas.microsoft.com/office/mailappversionoverrides"),
                    new XAttribute(xsi+"type", "VersionOverridesV1_0"),
                    new XElement("Requirements",
                        new XElement(bt+"Sets",
                            new XAttribute("DefaultMinVersion", "1.3"),
                            new XElement("Set", 
                                new XAttribute("Name", "Mailbox")
                            )
                        )
                    ),
                    new XElement("Hosts",
                        new XElement("DesktopFormFactor",
//                            new XElement("FunctionFule",
//                                new XAttribute("resid", "functionFile")
//                            ),
                            new XElement("ExtensionPoint",
                                new XAttribute(xsi+"type", "AppointmentOrganizerCommandSurface"),
                                new XElement("OfficeTab",
                                    new XAttribute("id", "TabDefault"),
                                    new XElement("Group",
                                        new XAttribute("id", "msgReadGroup"),
                                        new XElement("Label",
                                            new XAttribute("resid", "groupLabel")
                                        ),
                                        new XElement("Control",
                                            new XAttribute(xsi+"type", "Button"),
                                            new XAttribute("id", "msgReadOpenPaneButton"),
                                            new XElement("Label",
                                                new XAttribute("resid", "paneReadButtonLabel")
                                            ),
                                            new XElement("Supertitle",
                                                new XElement("Title",
                                                    new XAttribute("resid", "paneReadSuperTipTitle")
                                                ),
                                                new XElement("Description",
                                                    new XAttribute("resid", "paneReadSuperTipDescription")
                                                )
                                            ),
                                            new XElement("Icon",
                                                new XElement(bt+"Image", 
                                                    new XAttribute("size", "16"),
                                                    new XAttribute("resid", "icon16")
                                                ),
                                                new XElement(bt+"Image", 
                                                    new XAttribute("size", "32"),
                                                    new XAttribute("resid", "icon32")
                                                ),
                                                new XElement(bt+"Image", 
                                                    new XAttribute("size", "80"),
                                                    new XAttribute("resid", "icon80")
                                                )
                                            ),
                                            new XElement("Action",
                                                new XAttribute(xsi+"type", "ShowTaskpane"),
                                                new XElement("SourceLocation", 
                                                    new XAttribute("resid", "messageReadTaskPanelUrl"))
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    ),
                    new XElement("Resources",
                        new XElement(bt+"Images",
                            new XElement(bt+"Image",
                                new XAttribute("id", "icon16"),
                                new XAttribute("DefaultValue", $"{_coreHelper.GetCore().GetSdkSetting(Settings.httpServerAddress)}/assets/icon-16.png")
                            ),
                            new XElement(bt+"Image",
                                new XAttribute("id", "icon32"),
                                new XAttribute("DefaultValue", $"{_coreHelper.GetCore().GetSdkSetting(Settings.httpServerAddress)}/assets/icon-32.png")
                            ),
                            new XElement(bt+"Image",
                                new XAttribute("id", "icon80"),
                                new XAttribute("DefaultValue", $"{_coreHelper.GetCore().GetSdkSetting(Settings.httpServerAddress)}/assets/icon-80.png")
                            )
                        ),
                        new XElement(bt+"Urls",
//                            new XElement(bt+"Url",
//                                new XAttribute("id", "functionFile"),
//                                new XAttribute("DefaultValue", $"{_coreHelper.GetCore().GetSdkSetting(Settings.httpServerAddress)}/function-file/function-file.html")
//                            ),
                            new XElement(bt+"Url",
                                    new XAttribute("id", "messageReadTaskPaneUrl"),
                                    new XAttribute("DefaultValue", $"{_coreHelper.GetCore().GetSdkSetting(Settings.httpServerAddress)}")
                            )
                        ),
                        new XElement(bt+"ShortStrings",
                            new XElement(bt+"String",
                                new XAttribute("id", "groupLabel"),
                                new XAttribute("DefaultValue", "Microting")
                            ),new XElement(bt+"String",
                                new XAttribute("id", "customTabLabel"),
                                new XAttribute("DefaultValue", "Microting")
                            ),new XElement(bt+"String",
                                new XAttribute("id", "paneReadButtonLabel"),
                                new XAttribute("DefaultValue", "Schedule eForm")
                            ),new XElement(bt+"String",
                                new XAttribute("id", "paneReadSuperTipTitle"),
                                new XAttribute("DefaultValue", "schedule crane/work")
                            )
                        ),
                        new XElement(bt+"LongStrings",
                            new XElement(bt+"String",
                                new XAttribute("id", "paneReadSuperTipDescription"),
                                new XAttribute("DefaultValue", "Schedule eForm add-in")
                            )
                        )
                    )
                )
            );
            return xDocument;
        }
    }
}