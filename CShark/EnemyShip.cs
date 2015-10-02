using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    class EnemyShip : Ship
    {
        public EnemyShip(Vector3 pos, TgcMesh mesh) : base(pos, mesh)
        {
           
        }

        public void actualizar(float elapsedTime)
        {
            //por el momento lo dejo vacio
            
        }
    }
}
