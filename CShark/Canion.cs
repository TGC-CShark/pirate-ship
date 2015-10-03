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
    class Canion
    {
        public TgcMesh meshCanion;
        private List<Bala> balas = new List<Bala>();
        private Bala currentBullet;
        public int cantBalas = 50;
        static TgcD3dInput input = GuiController.Instance.D3dInput;

        public float anguloRotacion;
        public float anguloElevacion = 45;

        public Vector3 posicion;

        public Canion(Vector3 pos, TgcMesh mesh)
        {
            meshCanion = mesh;
            meshCanion.Position = pos;
            posicion = pos;

            for (int i = 0; i < cantBalas; i++)
            {
                Bala bullet = new Bala(pos);
                this.balas.Add(bullet);

            }
            currentBullet = balas.First();

            mesh.AutoTransformEnable = false;
        }

        public void shoot(float elapsedTime, float anguloRotacion)
        {
            currentBullet.bullet.Position = posicion;
            currentBullet.posicion = posicion;
            currentBullet.visible = true;
            currentBullet.anguloRotacion = anguloRotacion;
            currentBullet.anguloElevacion = anguloElevacion;
            currentBullet.verticalSpeed = currentBullet.verticalInitialSpeed;
            
            currentBullet = balas.ElementAt(balas.IndexOf(currentBullet)+1); 
        }

        public void render()
        {
            meshCanion.render();

            foreach (Bala bala in balas)
            {
               bala.render();
               
            }
        }

        internal void dispose()
        {

            meshCanion.dispose();
            foreach (Bala bala in balas)
            {
                bala.dispose();
            }
        }

        internal void actualizar(float anguloRotacion, float elapsedTime, Vector3 pos)
        {
            this.anguloRotacion = anguloRotacion;
            posicion = pos;

            if (input.keyPressed(Key.Space))
            {
                shoot(elapsedTime, anguloRotacion);
            }

            foreach (Bala bala in balas)
            {
                if (bala.visible)
                {
                    bala.dispararParabolico(elapsedTime);
                }
            }
        }
    }
}
