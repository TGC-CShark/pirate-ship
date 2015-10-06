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

        Ship player;

        const float ROTATION_SPEED = 1f;
        const float VEL_MAXIMA = 500f;
        const float ESCALON_VEL = 0.4f;

        public EnemyShip(Ship player, Vector3 pos, TgcMesh mesh, Canion canion) : base(pos, mesh, canion) 
        { 
            nombre = "EnemyShip";
            this.player = player;
            anguloRotacion = FastMath.PI;
        }

        public override void calcularTraslacionYRotacion(float elapsedTime)
        {
            float delta = player.AnguloRotacion - this.AnguloRotacion;

            if (FastMath.Abs(delta) > FastMath.PI / 2)
            {
                float correcionGiro = elapsedTime * ROTATION_SPEED;
                anguloRotacion = delta < 0 ? anguloRotacion - correcionGiro : anguloRotacion + correcionGiro;
                    
                rotacion = Matrix.RotationY(anguloRotacion);
            }

            //Cargar valor en UserVar
            GuiController.Instance.UserVars.setValue("angulo IA", anguloRotacion);
        }

        public override void actualizarCanion(float rotacion, float elapsedTime, Vector3 newPosition)
        {

        }

    }
}
