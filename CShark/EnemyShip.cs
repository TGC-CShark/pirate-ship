using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    public class EnemyShip : Ship
    {
        BarraVidaEnemigo barraDeVidaEnemigo;

        public EnemyShip(Vector3 pos, TgcMesh mesh, Canion canion) : base(pos, mesh, canion) {
            nombre = "ENEMIGO";
            iniciarBarra();
            //barraDeVida.nombre.Align = TgcText2d.TextAlign.RIGHT;
        }


        public void actualizar(float elapsedTime, TerrenoSimple agua, float time)
        {
          
            Matrix transformacionAgua = calcularPosicionConRespectoAlAgua(agua, elapsedTime, time);

            mesh.Transform = transformacionAgua;
            canion.meshCanion.Transform = transformacionAgua;


        }

        public void iniciarBarra()
        {
            barraDeVidaEnemigo = new BarraVidaEnemigo(new Vector2(0, 0), nombre);
        }

        internal void dispose()
        {

            mesh.dispose();
            canion.dispose();
            barraDeVidaEnemigo.dispose();
        }

        public void renderizar()
        {

            if (tieneVida())
            {
                mesh.render();
                canion.render();
                barraDeVidaEnemigo.render();
            }

        }

        private void reducirVida()
        {
            //vida -= 1;
            barraDeVidaEnemigo.escalar(porcentajeDeVida());
            GuiController.Instance.Logger.log("Vida contrincante: " + vida.ToString());
        }
    }
}
