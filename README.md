# RE4-PS2-BIN-TOOL
Extract and repack ps2 re4 bin files

Translate from Portuguese Brazil

Esse é o código resultado a partir das pesquisas sobre os arquivos .bin do re4 de ps2, realizadas por Jaderlink em conjunto com o HardRain.
Encontra-se em estágio de desenvolvimento, mas já pode ser usado para criar novos .bin para os cenários e os itens, e agora com suporte para peso por bone, pode criar personagens personalizados e armas com animações.

----
<br>**Update: alfa.1.2.0.0**

Agora temos suporte ao modelo 3d SMD (StudioModel Data), com suporte aos bones e pintura de peso por vértice. Porem os arquivos bin gerados pelo smd sera maior que os gerados pelos obj, pois esse formato ainda não foi feito o suporte a compressão de vértices (Triangle Strip), no qual se encontra em desenvolvimento.

**Aviso de compatiblidade dos arquivos entre as versões:**

 - Arquivo .IDXBIN: a versão alfa.1.2.0.0 e alfa.1.0.0.3 são compatíveis, qualquer versão anterior a esse é incompatível e o arquivo deve ser substituído pela versão atual (reentrai-a o arquivo bin)

 - Arquivo .OBJ: a versão alfa.1.2.0.0 e alfa.1.0.0.3 são compatíveis, os .obj dessas duas versões são 100x menor que as versões anteriores, esse é o novo padrão que segue a mesma escala dos modelos do "Son of Percia" (RE4 UHD). Mas casos você tenha um arquivo obj gerado por uma versão anterior a alfa.1.0.0.3, mude o campo "GlobalScale" no arquivo .idxbin de 100 para 1; 

- arquivo .SMD: use o arquivo gerado a partir da versão alfa.1.2.0.0, não use o arquivo smd gerado na versão alfa.1.0.0.3 pois esta errado. (a orientação das faces estava invertidas).

- arquivo .MTL: é usado para carregar as texturas do modelo nos editores 3d (ex: Blender), não é usado pelo "BINrepackTest", então ele não faz diferença.

- arquivo .TXT2: são arquivos informacionais, você pode ignora-los.



**Código de terceiro:**

[ObjLoader by chrisjansson](https://github.com/chrisjansson/ObjLoader/).
Encontra-se no BINrepackTest, código modificado, não considera mais a tag "g", os grupos são criados a partir da tag de material "usemtl" 

  ## BINdecoderTest

Decodifica e cria um .obj/.smd do bin.
São criados os arquivos .txt2 .obj .smd .mtl e .idxbin
  - sendo os arquivos txt2 arquivos informativos que divide o bin por partes para melhor visualização, que por padrão não são gerados.
  - .MTL: arquivo que acompanha o .obj serve para carregar as texturas nos modelos, porem as texturas devem ser extraídas do .TPL, e enumeradas a partir de zero.
  - .IDXBIN: é o arquivo que contem o conteúdo que não esta no .obj ou no .smd, mas é necessário para recompilar o .BIN. (contem o conteúdo do "material"/"Id da textura", veja a seção abaixo)
  - os bin de cenários possuem cores em vez de normals, agora ao recompilar não terá mais vertices coloridas.
  - Com a opção de arquivos de debug ativo, serão gerados mais 2 arquivos .obj: ".DrawDistanceBox.obj" e "ScaleLimitBox.obj", que representam as coordenados do DrawDistance e o outro o limite da escala do modelo extraído.
  - .OBJ e .SMD: contem o conteúdo do modelo 3d.

## BINrepackTest

Cria um novo arquivo .BIN a partir do conteúdo do .IDXBIN e .OBJ ou .SMD
  - O programa recebe como entrada um aquivo .obj ou .smd, e também deve ter um arquivo idxbin na mesma pasta com o mesmo nome do arquivo fornecido. 
  - A quantidade de materiais do bin é definido pelo arquivo .obj/.smd, os materiais serão colocados em ordem alfabética no arquivo bin.
  - por exemplo: caso o arquivo .obj/.smd tenha 4 materiais, no arquivo .idxbin deve conter 4 entradas cada com a tag "materialLine_*", sendo o * o nome do material presente no arquivo .obj/.smd. (veja mais na seção abaixo)
  - A compressão das vértices agora é funcional, deve funcionar bem (porem atualmente so funciona para arquivos .obj), para ativar a função o arquivo .idxbin deve conter a tag "CompressVertices:True", caso não queira comprimir as vértices mude "True" para "False".
  - Adicionado "auto fator de conversão", com a função ativa o programa vai determinar o fator automaticamente, aproveitando o máximo das coordenadas do "short" mantendo o modelo mais fidedigno ao original, para ativar essa função o arquivo .idxbin deve conter a tag "AutoConversionFactor:True", caso for false, ele irá usar o valor da tag "ManualConversionFactor". (não confundir essa função com a escala do modelo).
  - Alguns pequenos bugs que crashavam o programa foram corrigidos.

## Materials/Texturas no .IDXBIN

Aqui é uma explicação de como associar corretamente o id da textura ao modelo reempacotado:
<br>As texturas ficam no arquivo .TPL e seus IDs são enumerados a partir de 0 (zero).
<br>Para fazer a associação do id da textura com o material usado no modelo é usado o arquivo .IDXBIN:
<br>
<br> Os nomes dos materiais nos arquivos .obj é definido pela tag "usemtl" (ex: usemtl MATERIAL_000);
<br>Nos arquivos .smd os nomes dos materiais ficam acima do conjunto de dados de cada triangulo (que fica depois da tag "triangles", e a cada 3 linhas é um nome de material)
<br> e No arquivo >IDXBIN deve conter o mesmo nome de material usado no .obj/.smd:


>MaterialLine?MATERIAL_000:0000FFFFFF000000000000FF
<br>MaterialLine?MATERIAL_001:0001FFFFFF000000000000FF

Depois de ? e antes de : é o nome do material.
<br>Depois de : é uma sequencia de 12 bytes sendo cada byte representado por 2 caracteres.
<br>Sendo o segundo byte o id da textura a ser usado. Ex:
<br>**00XXFFFFFF000000000000FF**
<br> No lugar de XX coloque o id da textura a ser usada (que esteja no arquivo .TPL);
<br> Caso você for usar um novo material, você pode usar a sequencia de bytes acima.
<br> Os outros bytes não connhecemos a função.
<br> caso um material no arquivo .obj/.smd não for fornecido, sera usado a sequencia padão que é a sequinte: 0000FFFFFF000000000000FF

## Ordem dos bones no arquivo .SMD

Para arrumar a ordem dos ids dos bones nos arquivos smd, depois de serem exportados do blender ou outro software de edição de modelos usar o programa: GC_GC_Skeleton_Changer.exe (procure o programa no fórum do re4)

## Carregando as texturas no arquivo .SMD

No blender para carregar o modelo .SMD com as texturas, em um novo "projeto", importe primeiro o arquivo .obj para ele carregar as texturas, delete o modelo do .obj importado, agora importe o modelo .smd, agora ele será carregado com as texturas.
<br>Lembrando também que tem que extrair as texturas do TPL, com o programa "TPL_PS2_EXTRACT.exe", e colocar a pasta com o nome "Textures" junto do arquivo .MTL

At.te: JADERLINK
<br>2023-08-19