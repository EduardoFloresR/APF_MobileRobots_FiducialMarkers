%% Load STL mesh
% Stereolithography (STL) files are a common format for storing mesh data. STL
% meshes are simply a collection of triangular faces. This type of model is very
% suitable for use with MATLAB's PATCH graphics object.

%% Load STL mesh
fv = stlread('planoInclinado.stl');

%% Escalar las coordenadas al rango [0, 1]
% Encuentra los valores mínimos y máximos en cada dimensión
min_values = min(fv.vertices);
max_values = max(fv.vertices);

% Calcula el rango en cada dimensión
range_values = max_values - min_values;

% Escala las coordenadas al rango [0, 100]
scaled_vertices = 100*(fv.vertices - min_values) ./ range_values;

% Actualiza las coordenadas en la estructura fv
fv.vertices = scaled_vertices;

%% Render
patch(fv, 'FaceColor', [0.8 0.8 1.0], ...
         'EdgeColor', 'none', ...
         'FaceLighting', 'gouraud', ...
         'AmbientStrength', 0.15);

camlight('headlight');
material('dull');

title('Archivo STL');
axis('image');
xlabel('X');
ylabel('Y');
zlabel('Z');
view([-135 35]);

%% Grafica de la nube de puntos
% Extrae las coordenadas x, y, z de la estructura escalada
x = scaled_vertices(:, 1);
y = scaled_vertices(:, 2);
z = scaled_vertices(:, 3);

% Grafica los puntos en 3D
figure;
scatter3(x, y, z);
title('Nube de Puntos');
xlabel('X');
ylabel('Y');
zlabel('Z');
axis('image');
view([-135 35]);
grid on;

%% Fitted Model
fittedmodel = fit([x,y], z, 'linearinterp');

figure;
plot(fittedmodel); % Utiliza la función adecuada para graficar el modelo ajustado
title('Fitted Model');
axis('image');
xlabel('X');
ylabel('Y');
zlabel('Z');
view([-135 35]);

%% Gradient of Fitted Model

% Define el rango de valores para evaluar el gradiente
[x_range, y_range] = meshgrid(linspace(min(x), max(x), 100), ...
                                       linspace(min(y), max(y), 100));

% Combina las coordenadas en una matriz de entrada
coordinates = [x_range(:), y_range(:)];

% Evalúa el modelo ajustado en el rango definido
fitted_values = feval(fittedmodel, coordinates);

% Reformatea el resultado para que coincida con las dimensiones originales
fitted_values = reshape(fitted_values, size(x_range));

[px,py,pz] = surfnorm(x_range, y_range, fitted_values);
figure;
hold on
patch(fv,'FaceColor',       [0.8 0.8 1.0], ...
         'EdgeColor',       'none',        ...
         'FaceLighting',    'gouraud',     ...
         'AmbientStrength', 0.15);
quiver3(x_range, y_range, fitted_values, px,py,pz, 4);
axis('image');
title('Gradiente del Modelo Ajustado');
xlabel('X');
ylabel('Y');
zlabel('Z');
view([-135 35]);
grid on;

%% Fuerzas en el plano
figure;
quiver(x_range, y_range, px,py, 2);
axis('image');
title('Fuerzas en el plano');
xlabel('X');
ylabel('Y');
grid on;

%% 
% Combinar las matrices u y v en una sola matriz tridimensional
resultante = cat(3, px,py);
