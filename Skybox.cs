using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    class Skybox
    {
        Model skybox;
        //Model per face?
        public Skybox()
        {
            //Constructor
        }

        public void Draw(int matID)
        {
            skybox.Draw(matID);
            //howa m4 4art model skybox ba2a, n3mil model lkol Face
            //34an binding el-texture
        }
    }

}
