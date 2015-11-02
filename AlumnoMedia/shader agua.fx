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
float transparency = 0.9;
float height;

float radioBala;
float posBalaX;
float posBalaZ;

//Para la iluminación
float3 fvLightPosition = float3( -100.00, 100.00, -100.00 );
float3 fvEyePosition = float3( 0.00, 0.00, -100.00 );
float k_la = 0.3;							// luz ambiente global
float k_ld = 0.9;							// luz difusa
float k_ls = 0.4;							// luz specular
float fSpecularPower = 16.84;				// exponente de la luz specular

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
	float k = 6.2831853 / length; //2pi
	float3 K = (0.7854, 0, 0.7854);
	float w = sqrt(9.8 * k);
	height = height * 2;

	Input.Position.x = X;
	Input.Position.z = Z;
	Input.Position.y = height * (cos(0.005*X-time) + sin(0.005*Z-time));

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

float4 ps_main( float3 Texcoord: TEXCOORD0, float3 N:TEXCOORD1,
	float3 Pos: TEXCOORD2) : COLOR0
{      
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular
	
	N = normalize(N);

	// si hubiera varias luces, se podria iterar por c/u. 
	// Pero hay que tener en cuenta que este algoritmo es bastante pesado
	// ya que todas estas formulas se calculan x cada pixel. 
	// En la practica no es usual tomar mas de 2 o 3 luces. Generalmente 
	// se determina las luces que mas contribucion a la escena tienen, y 
	// el resto se aproxima con luz ambiente. 
	// for(int =0;i<cant_ligths;++i)
	// 1- calculo la luz diffusa
	float3 LD = normalize(fvLightPosition-float3(Pos.x,Pos.y,Pos.z));
	ld += saturate(dot(N, LD))*k_ld;
	
	// 2- calcula la reflexion specular
	float3 D = normalize(float3(Pos.x,Pos.y,Pos.z)-fvEyePosition);
	float ks = saturate(dot(reflect(LD,N), D));
	ks = pow(ks,fSpecularPower);
	le += ks*k_ls;

	//Obtener el texel de textura
	float4 fvBaseColor = tex2D( diffuseMap, Texcoord );
	//float4 fvBaseColor      = float4(1,0.5,0.5,1);
	
	// suma luz diffusa, ambiente y especular
	float4 RGBColor = 0;
	RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la+ld)) + le);
	
	// saturate deja los valores entre [0,1]. Una tecnica muy usada en motores modernos
	// es usar floating point textures auxialres, para almacenar mucho mas que 256 valores posibles 
	// de iluminiacion. En esos casos, el valor del rgb podria ser mucho mas que 1. 
	// Imaginen una excena outdoor, a la luz de sol, hay mucha diferencia de iluminacion
	// entre los distintos puntos, que no se pueden almacenar usando solo 8bits por canal.
	// Estas tecnicas se llaman HDRLighting (High Dynamic Range Lighting). 
	// Muchas inclusive simulan el efecto de la pupila que se contrae o dilata para 
	// adaptarse a la nueva cantidad de luz ambiente. 
	
	return RGBColor;
}

/*
//Pixel Shader
float4 ps_main( float2 Texcoord: TEXCOORD0, float3 N:TEXCOORD1,
	float3 Pos: TEXCOORD2, float4 Color:COLOR0) : COLOR0
{      
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular
	
	N = normalize(N);

	// 1- calculo la luz diffusa
	float3 LD = normalize(fvLightPosition-float3(Pos.x,Pos.y,Pos.z));
	ld += saturate(dot(N, LD))*k_ld;
	
	// 2- calcula la reflexion specular
	float3 D = normalize(float3(Pos.x,Pos.y,Pos.z)-fvEyePosition);
	float ks = saturate(dot(reflect(LD,N), D));
	ks = pow(ks,fSpecularPower);
	le += ks*k_ls;

	//Obtener el texel de textura
	float4 fvBaseColor = tex2D( diffuseMap, Texcoord );
	//float4 fvBaseColor      = float4(1,0.5,0.5,1);
	
	float4 retorno = 0.8*fvBaseColor + 0.2*Color;
	retorno.a = transparency;
	
	// suma luz diffusa, ambiente y especular
	float4 RGBColor = 0;
	//RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la+ld)) + le);
	RGBColor.rgb = saturate(retorno*(saturate(k_la+ld)) + le);
	
	// saturate deja los valores entre [0,1]. Una tecnica muy usada en motores modernos
	// es usar floating point textures auxialres, para almacenar mucho mas que 256 valores posibles 
	// de iluminiacion. En esos casos, el valor del rgb podria ser mucho mas que 1. 
	// Imaginen una excena outdoor, a la luz de sol, hay mucha diferencia de iluminacion
	// entre los distintos puntos, que no se pueden almacenar usando solo 8bits por canal.
	// Estas tecnicas se llaman HDRLighting (High Dynamic Range Lighting). 
	// Muchas inclusive simulan el efecto de la pupila que se contrae o dilata para 
	// adaptarse a la nueva cantidad de luz ambiente. 
	
	return RGBColor;
	
	// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
	//float4 fvBaseColor = tex2D( diffuseMap, Texcoord );
	// combino color y textura
	// en este ejemplo combino un 80% el color de la textura y un 20%el del vertice
	//float4 retorno = 0.8*fvBaseColor + 0.2*Color;
	//float4 retorno = 0.8*RGBColor + 0.2*Color;
	
	//retorno.a = transparency;
	//return retorno;
}*/

//Pixel Shader para la sombra de la bala
float4 ps_sombra( float2 Texcoord: TEXCOORD0, float4 Color:COLOR0) : COLOR0
{      
	float4 retorno = float4(0,0,0,0);	
	retorno.a = transparency;
	return retorno;
}


// ------------------------------------------------------------------
technique RenderScene
{
   pass Pass_0
   {
		//AlphaBlendEnable = TRUE;
        //DestBlend = INVSRCALPHA;
        //SrcBlend = SRCALPHA;
	  VertexShader = compile vs_2_0 vs_main();
	  PixelShader = compile ps_2_0 ps_main();
   }

}

technique SombraBala{
	pass Pass_0{
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
	  VertexShader = compile vs_2_0 vs_main();
	  PixelShader = compile ps_2_0 ps_sombra();
	}
}