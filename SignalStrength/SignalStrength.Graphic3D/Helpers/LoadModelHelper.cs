using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using HelixToolkit.Wpf;

namespace SignalStrength.Graphic3D.Helpers
{
    public class LoadModelHelper
    {
        /// <summary>
        /// Load models from path
        /// </summary>
        /// <param name="path">Full PathName file</param>
        /// <returns>Model3DGroup</returns>
        public static Model3DGroup LoadModel(string path)
        {
            Model3DGroup model3DGroup = new Model3DGroup();
            var importer = new ModelImporter();
            var mod = importer.Load(path);

            if (mod == null || mod.Children.Count == 0)
                return new Model3DGroup();

            foreach (var modelF in mod.Children)
            {
                var model = modelF as GeometryModel3D;
                model3DGroup.Children.Add(model);
            }
       
            return model3DGroup ;
        }
    }

   
}
