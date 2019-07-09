using System.Xml.Linq;

namespace Appointment.Pn.Abstractions
{
    public interface IOutlookAddinService
    {
        XElement Manifest();
    }
}