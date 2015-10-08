// ---------------------------------------------------------
// Ejemplo shader Minimo:
// ---------------------------------------------------------

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float time = 0;
float transparency = 0.7;
float height;


/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT 
{
   float4 Position : POSITION0;
   float4 Color : COLOR0;
   float2 Texcoord : TEXCOORD0;
   float3 Normal :   NORMAL0;
};

//Output del Vertex Shader
struct VS_OUTPUT 
{
   float4 Position :        POSITION0;
   float2 Texcoord :        TEXCOORD0;
   float4 Color :			COLOR0;
   float3 Norm :          TEXCOORD1;			// Normales
   float3 Pos :   		TEXCOORD2;		// Posicion real 3d
};



//Vertex Shader
VS_OUTPUT vs_main( VS_INPUT Input )
{
	VS_OUTPUT Output;

	float Y = Input.Position.y;
	float Z = Input.Position.z;
	float X = Input.Position.x;

	float length = 10;
	float3 k = 6.2831853 / length; //2pi
	float3 K = (0.7854, 0, 0.7854);
	float w = sqrt(9.8 * k);
	height = height * 2;

	Input.Position.x = X - K * height * (sin(0.25 * (K * k * X - w * time)) + cos(0.5 * (X - w * time)));
	Input.Position.z = Z - K * height * (sin((K * k * Z - w * time)) + (cos(Z - w * time)));
	Input.Position.y = height * (cos(0.25   * (K * k * X - w * time)) + sin(0.5   * (X - w * time)));

	   //Proyectar posicion
	   Output.Position = mul( Input.Position, matWorldViewProj);
   
	   //Propago las coordenadas de textura
	   Output.Texcoord = Input.Texcoord;

	   //Propago el color x vertice
	   Output.Color = Input.Color;
   
	   // Calculo la posicion real (en world space)
	   float4 pos_real = mul(Input.Position, matWorld);
	   // Y la propago usando las coordenadas de texturas
	   Output.Pos = float3(pos_real.x, pos_real.y, pos_real.z);
   
	   // Transformo la normal y la normalizo
	   Output.Norm = normalize(mul(Input.Normal, matWorld));

	return( Output );
   
}


// Ejemplo de un vertex shader que anima la posicion de los vertices 
// ------------------------------------------------------------------

//Pixel Shader
float4 ps_main( float2 Texcoord: TEXCOORD0, float4 Color:COLOR0) : COLOR0
{      
	// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
	float4 fvBaseColor = tex2D( diffuseMap, Texcoord );
	// combino color y textura
	// en este ejemplo combino un 80% el color de la textura y un 20%el del vertice
	float4 retorno = 0.8*fvBaseColor + 0.2*Color;
	retorno.a = transparency;
	return retorno;
}


// ------------------------------------------------------------------
technique RenderScene
{
   pass Pass_0
   {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
	  VertexShader = compile vs_2_0 vs_main();
	  PixelShader = compile ps_2_0 ps_main();
   }

}