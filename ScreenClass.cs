using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using System.Threading;

namespace Graphics {
    abstract class ScreenClass {
        public static string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

        public abstract void Initialize();
        public abstract void Draw();
        public abstract void CleanUp();
    }
}
