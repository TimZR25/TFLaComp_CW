using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFLaComp_CW.ParserHelp
{
    public interface IParserHelpProvider
    {
        public HelpProvider HelpProvider { get; }

        public void SetHelp(Control control, string keyword, HelpNavigator helpNavigator);
    }
}
