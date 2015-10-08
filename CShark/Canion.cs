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
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    public class Canion
    {
        public TgcMesh meshCanion;

        public int balasRestantes = 50;
        static TgcD3dInput input = GuiController.Instance.D3dInput;
        public List<Bala> balasEnElAire = new List<Bala>();
        public float anguloRotacion;
        public float anguloElevacion = 45f;
        const float ELEVACION_MAX = 90f;
        const float ELEVACION_MIN = 0f;
        public bool elevacion_visible = false;
        public TgcText2d texto_elevacion;

        public Vector3 posicion;

        public Canion(Vector3 pos, float offsetShip, TgcMesh mesh)
        {
            meshCanion = mesh;
            meshCanion.Position = new Vector3(0, offsetShip, 0) + pos;
            posicion = new Vector3(0, offsetShip, 0) + pos;

            mesh.AutoTransformEnable = false;

            texto_elevacion = new TgcText2d();
            texto_elevacion.Text = anguloElevacion.ToString() + "º";
            texto_elevacion.Color = Color.Gold;
            texto_elevacion.Align = TgcText2d.TextAlign.CENTER;
            texto_elevacion.Position = new Point(200, 100);
            texto_elevacion.Size = new Size(800, 300);
            texto_elevacion.changeFont(new System.Drawing.Font("BlackoakStd", 35, FontStyle.Bold | FontStyle.Italic));

        }

        public void shoot(float elapsedTime, float anguloRotacion)
        {
            new Bala(posicion, anguloRotacion, anguloElevacion, this);
            balasRestantes--;
            
        }

        public void render()
        {
            meshCanion.render();

            if (elevacion_visible)
            {
                texto_elevacion.render();
            }

        }

        internal void dispose()
        {

            meshCanion.dispose();
        }

        internal void actualizar(float anguloRotacion, float elapsedTime, Matrix transformacion)
        {
            this.anguloRotacion = anguloRotacion;
            meshCanion.Transform = transformacion;
            posicion = new Vector3(meshCanion.Transform.M41,meshCanion.Transform.M42,meshCanion.Transform.M43);

            


        }

        public void actualizarSiEsJugador(float anguloRotacion, float elapsedTime, Matrix transformacion)
        {

            if (input.keyPressed(Key.A))
            {
                if (elevacion_visible)
                {
                    elevacion_visible = false;
                } else
                {
                    elevacion_visible = true;
                }
            }

            if (input.keyPressed(Key.Space))
            {
                shoot(elapsedTime, anguloRotacion);
            }

            if (input.keyDown(Key.W))
            {
                incrementarAnguloElevacion(elapsedTime);
            }

            if (input.keyDown(Key.S))
            {
                decrementarAnguloElevacion(elapsedTime);
            }


            //foreach (Bala bala in balasEnElAire)
            for (int i = 0; i < balasEnElAire.Count; i++)
            {
                Bala bala = balasEnElAire[i];
                bala.actualizar(elapsedTime);
                bala.render();
            }
        }

        private void decrementarAnguloElevacion(float elapsedTime)
        {
           
            float aux = anguloElevacion;
            anguloElevacion = Math.Max(ELEVACION_MIN, aux-1);
            
            texto_elevacion.Text = anguloElevacion.ToString() + "º";
        }

        private void incrementarAnguloElevacion(float elapsedTime)
        {
            
            float aux = anguloElevacion;
            anguloElevacion = Math.Min(ELEVACION_MAX, aux + 1);
            
            texto_elevacion.Text = anguloElevacion.ToString() + "º";
        }

        public void agregarBalaEnElAire(Bala bala)
        {
            this.balasEnElAire.Add(bala);
        }

        public void eliminarBalaEnElAire(Bala bala)
        {
            this.balasEnElAire.Remove(bala);
        }
    }
}
