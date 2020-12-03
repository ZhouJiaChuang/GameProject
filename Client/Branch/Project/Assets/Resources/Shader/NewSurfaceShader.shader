Shader "ZJC_TEST/NewSurfaceShader"
{
	properties{
		_Color("Main Color", color) = (1,1,1,1)
		_Ambient("Ambient", color) = (0.3,0.3,0.3,0.3)
		_Specular("Specular", color) = (1,1,1,1)
		_Shininess("Shininess", range(0,8)) = 4
		_Emission("Emission", color) = (1,1,1,1)

		_MainTex("MainTex", 2d) = ""
		_SecondTex("SecondTex", 2d) = ""
	}

	SubShader
	{

		pass {
			//color(1,0.5,0.5,0.5)
			//color[_Color]
			material
			{
				diffuse[_Color]
				//环境光
				ambient[_Ambient]
				//高光
				specular[_Specular]
				//高光范围
				shininess[_Shininess]
				//自发光
				emission[_Emission]
			}
			lighting on
			separatespecular on

			settexture[_MainTex]
			{
				combine texture * primary double
			}

			settexture[_SecondTex]
			{
				combine texture * previous
			}
		}

	}
}
