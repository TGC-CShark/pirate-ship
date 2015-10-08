using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    public class Canion
    {
        public TgcMesh meshCanion;

        public int balasRestantes= 50;
        static TgcD3dInput input = GuiController.Instance.D3dInput;
        public List<Bala> balasEnElAire = new List<Bala>();
        public float anguloRotacion;
        public float anguloElevacion = 45;

        public Vector3 posicion;

        public Canion(Vector3 pos, float offsetShip, TgcMesh mesh)
        {
            meshCanion = mesh;
            meshCanion.Position = new Vector3(0, offsetShip, 0) + pos;
            posicion = new Vector3(0, offsetShip, 0) + pos;

            mesh.AutoTransformEnable = false;
        }

        public void shoot(float elapsedTime, float anguloRotacion)
        {
            new Bala(posicion, anguloRotacion, anguloElevacion, this);
            balasRestantes--;
            
        }

        public void render()
        {
            meshCanion.render();

            

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

            if (input.keyPressed(Key.Space))
            {
                shoot(elapsedTime, anguloRotacion);
            }

            //foreach (Bala bala in balasEnElAire)
            for (int i = 0; i < balasEnElAire.Count; i++)
            {
                Bala bala = balasEnElAire[i];
                bala.actualizar(elapsedTime);
                bala.render();
            }
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
