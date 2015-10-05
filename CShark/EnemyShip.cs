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
        

        public EnemyShip(Vector3 pos, TgcMesh mesh, Canion canion) : base(pos, mesh, canion) { nombre = "EnemyShip"; }


        public void actualizar(float elapsedTime, TerrenoSimple agua, float time)
        {
            
            //por el momento lo dejo vacio
            traslacion = Matrix.Translation(movX, 0, movZ);

            mesh.Transform = traslacion;
            //canion.meshCanion.Transform = traslacion;
            

            /*calcularTraslacionYRotacion(elapsedTime);

            Matrix transformacion = rotacion * traslacion;

            mesh.Transform = transformacion;
            canion.meshCanion.Transform = transformacion;*/
        }
    }
}
