#version 130

struct Light
{
        vec4	type;		// тип источника
        vec4	position;	// позиция источника (есть у Point и Spot)
        vec4	direction;	// направление света
        vec4	color;		// (цвет.r, цвет.g, цвет.b, интенсивность)
};

const int MAX_LIGHT_COUNT = 3;

// Shader parameters
uniform mat4 matWorldNormal;
uniform mat4 matWorldT;
uniform vec4 materialColor;
uniform vec4 lightsCount;
uniform vec4 cameraPosition;
uniform Light lights[MAX_LIGHT_COUNT];

varying vec3 localPosition;
varying vec3 localNormal;


vec3 calcDiffuse(vec4 lightCol, vec3 lightDir, vec3 vertexNormal)
{
        // dot = dot product of vectors (scalar multiplication)
        //clamp - may be cutOffBorders
        //diffuse - it is only one value (not vector) from [0, 1]
        float diffuse = clamp( dot(-lightDir, vertexNormal), 0.0, 1.0 );

        // Цвет = diffuse * (цвет источника) * (интенсивность источника)
        vec3 color = diffuse * lightCol.rgb * lightCol.a;
        //lightCol.rgb - has vec3 type

        return color;
}

void main()
{
        vec3 col = vec3(0,0,0);

        // Переводим нормаль из локальной в глобальную систему координат
        vec3 vertexNormal = (vec4(localNormal, 0.0) * matWorldNormal).xyz;
        vertexNormal = normalize(vertexNormal);

        // Переводим позицию из локальной в глобальную систему координат
        vec3 vertexPos = (vec4(localPosition, 1.0) * matWorldT).xyz;

        for (int i = 0; float(i) < lightsCount.x; ++i)
        {
                // Выходим, как только закончились источники освещения
                //if (float(i) >= lightsCount.x) break;

                float type = lights[i].type.x;
                float epsilon = 0.001;

                vec4 lightCol = lights[i].color;
                vec3 lightDir = vec3(0,0,0);

                float intensity = 1.0;
                float specular = 1.0;

                // Directional light
                if (abs(type - 1.0) < epsilon)
                {
                        lightDir = normalize(lights[i].direction.xyz).xyz;
                        // Specular component
                        vec3 halfWay = normalize(normalize(cameraPosition.xyz - vertexPos) + -lightDir);
                        specular = pow(
                            clamp(dot(vertexNormal, halfWay), 0.0, 1.0),
                            1000 );
                }
                // Point light
                else if (abs(type - 2.0) < epsilon)
                {
                        lightDir = normalize(vertexPos - lights[i].position.xyz).xyz;
                        // Specular component
                        vec3 halfWay = normalize(normalize(cameraPosition.xyz - vertexPos) + -lightDir);
                        specular = pow(
                            clamp(dot(vertexNormal, halfWay), 0.0, 1.0),
                            1000 );
                }
                // Spot light
                else if (abs(type - 3.0) < epsilon)
                {
                    // Diffuse component
                        lightDir = normalize(vertexPos - lights[i].position.xyz).xyz;
                        float spotDiffAngle = clamp(dot(lights[i].direction.xyz, lightDir), 0.0, 1.0);
                        const float SPOT_INNER_CONE = 0.9;      // #hardcode
                        const float SPOT_OUTER_CONE = 0.7;
                        intensity = clamp(1.0
                            - (SPOT_INNER_CONE - spotDiffAngle) / (SPOT_INNER_CONE - SPOT_OUTER_CONE)
                            , 0.0, 1.0);

                    // Specular component
                        vec3 halfWay = normalize(normalize(cameraPosition.xyz - vertexPos) + -lightDir);
                        specular = pow(
                            clamp(dot(vertexNormal, halfWay), 0.0, 1.0),
                            1000 );

                }
                col += materialColor.rgb * calcDiffuse(lightCol, lightDir, vertexNormal) * intensity;
                col += materialColor.rgb * specular * intensity;
        }

        gl_FragColor = vec4(col, 1.0);
        gl_FragColor.a = 1.0;       //why so?
}
