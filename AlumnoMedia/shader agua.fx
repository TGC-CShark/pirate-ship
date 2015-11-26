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
float transparency;// = 0.9;
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

//Para el alpha blending
texture texHeightmap;
sampler2D heightmap = sampler_state
{
	Texture = (texHeightmap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};
float offset;
float menorAltura;

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
	float Alpha :		COLOR1;
   };


float3 CalculoAltura(float x, float z) {

	float y = height * ( cos(0.005 * x - time) + sin(0.005 * z - time) );
	//float y = height;
	return float3(x, y, z);
}

float3 CalculoNormal(float3 pos) {

	float delta = 1;

	float3 vecino1 = CalculoAltura(pos.x + delta, pos.z);
	float3 vecino2 = CalculoAltura(pos.x, pos.z + delta);

	//vecino1 = mul(vecino1, matWorldViewProj).xyz;
	//vecino2 = mul(vecino2, matWorldViewProj).xyz;

	float3 tg = vecino1 - pos;
	float3 bitg = vecino2 - pos;

	return normalize(cross(bitg, tg));
}

float3 Gradiente(float3 pos){
	float x=pos.x;
	float z=pos.z;
	return (height*(-sin(0.005*x-time))*0.005,1,height*(cos(0.005*z-time)*0.005));
}
/*
float3 CalculoNormal(float3 pos) {
	return normalize(Gradiente(pos));
}
*/


//Vertex Shader
VS_OUTPUT vs_main( VS_INPUT input )
{
	
	VS_OUTPUT output;

	//Calcular el alpha blending de acuerdo a la altura del agua con respecto al heightmap
/*
	float4 posHeightmap = tex2Dlod(heightmap, float4(input.Position.xy/64000,0,0));
	input.Position.y = 4000*(posHeightmap.r * 0.299 + posHeightmap.g * 0.587 + posHeightmap.b * 0.114) - 1625;
	//float difAltura = input.Position.y + offset - posHeightmap;
	//output.Alpha = abs(difAltura / menorAltura);
*/
	//Esta variable es donde quiero cargar el valor de alfa de cada posición
	//Como todavía no funciona bien, lo cargo con la constante que teníamos
	output.Alpha = 0.9;
	
	//Calculo vertice desplazado por la ola
	input.Position = float4(CalculoAltura(input.Position.x, input.Position.z), 1);
	//Lo transformo en salida
	output.Position = mul(input.Position, matWorldViewProj);
	
	//Calculo normal
	output.Norm = CalculoNormal(input.Position.xyz);

	//Propago las coordenadas de textura
	output.Texcoord = input.Texcoord;
	//Propago el color x vertice
	output.Color = input.Color;
   
	// Calculo la posicion real (en world space)
	output.Pos = mul(input.Position, matWorld).xyz;

	return output;
}


// Ejemplo de un vertex shader que anima la posicion de los vertices 
// ------------------------------------------------------------------

//Pixel Shader
float4 ps_main( float2 Texcoord: TEXCOORD0, float3 N:TEXCOORD1,
	float3 Pos: TEXCOORD2, float4 Color:COLOR0, float alpha:COLOR1) : COLOR0
{      
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular
	
	// 1- calculo la luz diffusa
	float3 LD = normalize(fvLightPosition-float3(Pos.x,Pos.y,Pos.z));
	ld += saturate(dot(N, LD))*k_ld;
	
	// 2- calcula la reflexion specular
	float3 D = normalize(float3(Pos.x,Pos.y,Pos.z)-fvEyePosition);
	float ks = saturate(dot(reflect(LD,N), D));
	ks = pow(ks,fSpecularPower);
	le += ks*k_ls;

	//Obtener el texel de textura
	//Texcoord.x += 0.5+cos(Texcoord.x*time*0.05)/2;
	Texcoord.x += 0.1*cos(time*0.01);
	/*if (Texcoord.x > 1){
		Texcoord.x -= 1;
	}*/
	//Texcoord.y += 0.5+cos(Texcoord.y*time*0.05)/2;
	Texcoord.y += 0.1*cos(time*0.01);
	/*if (Texcoord.y > 1){
		Texcoord.y -= 1;
	}*/
	float4 fvBaseColor = tex2D( diffuseMap, Texcoord );
	//float4 fvBaseColor      = float4(1,0.5,0.5,1);

	// suma luz diffusa, ambiente y especular
	float4 RGBColor = 0;
	RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la+ld)) + le);
	
	// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
	// combino color y textura
	// en este ejemplo combino un 80% el color de la textura y un 20%el del vertice
	float4 retorno = 0.8*RGBColor + 0.2*Color;

	//retorno = fvBaseColor;
	return float4(retorno.r, retorno.g, retorno.b, alpha);
	//return float4(N,1);
}

//Pixel Shader para la sombra de la bala
float4 ps_sombra( float2 Texcoord: TEXCOORD0, float4 Color:COLOR0) : COLOR0
{      
	float4 retorno = float4(0,0,0,0.5);	
	Color.a = transparency;
	return retorno;
}

VS_OUTPUT vs_salpicadura( VS_INPUT input )
{	
	VS_OUTPUT output;
	
	//Calculo normal
	output.Norm = normalize(float3(input.Position.x, 0, input.Position.z));
	
	input.Position.xz = output.Norm.xz * time;
	float altmax = 100;
	//input.Position.y = altmax * sin(0.005*time);

	//Lo transformo en salida
	output.Position = mul(input.Position, matWorldViewProj);

	output.Alpha = 0.75;
	
	//Propago las coordenadas de textura
	output.Texcoord = input.Texcoord;
	//Propago el color x vertice
	output.Color = input.Color;
   
	// Calculo la posicion real (en world space)
	output.Pos = mul(input.Position, matWorld).xyz;

	return output;
}

float4 ps_salpicadura( float2 Texcoord: TEXCOORD0, float4 Color:COLOR0) : COLOR0
{      
	float4 fvBaseColor = tex2D( diffuseMap, Texcoord );
	float4 retorno = 0.8*fvBaseColor + 0.2*Color;//float4(0.2,0.7,0.9,0.2);
	retorno.a = 0.5;
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
	  VertexShader = compile vs_3_0 vs_main();
	  PixelShader = compile ps_2_0 ps_main();
   }

}

technique SombraBala{
	pass Pass_0{
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
	  VertexShader = compile vs_3_0 vs_main();
	  PixelShader = compile ps_2_0 ps_sombra();
	}
}

technique SalpicaduraBala{
	pass Pass_0{
	AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
	  VertexShader = compile vs_2_0 vs_salpicadura();
	  PixelShader = compile ps_2_0 ps_main();
	}
}