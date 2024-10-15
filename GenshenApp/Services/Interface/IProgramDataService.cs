using GenshenApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenshenApp.Services.Interface
{
    public interface IProgramDataService
    {
        public ProgramSettingData SettingData { get; set; }
        public ProgramData ProgramData { get; set; }

        public void LoadSettingData();
        public Task LoadProgramData();
    }
}
