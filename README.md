# RE4-PS2-BIN-TOOL
Extract and repack ps2 re4 bin files

Translate from Portuguese Brazil

Esse é o código resultado a partir das pesquisas sobre os arquivos .bin do re4 de ps2 em conjunto com o HardRain.
Encontra-se em estágio de desenvolvimento, mas já pode ser usado para criar novos .bin para os cenários e os itens.

----

<br>**Update: alfa.1.0.0.3**

**Aviso3: os arquivos da versão alfa.1.0.0.2 são incompatíveis com a versão alfa.1.0.0.3 do programa.**

 - Nesse update a escala dos modelos foi reduzida em 100x ao tamanho anterior, caso queria usar um arquivo .obj da verão anterior, será necessário mudar a tag "GlobalScale" de 100 para 1
 - O arquivo .idxbin mudou, então o arquivo das versões anteriores são incompatíveis.
- Agora o conteúdo do material é identificado pelo nome do material que esta no arquivo .obj/.smd
- Nota: esta sendo gerado um arquivo .smd, porem ainda não foi adicionado no "BINrepackTest" a funcionalidade de criar um arquivo .bin a partir desse arquivo, estará disponível em uma próxima versão.
- Mais informações são adicionadas na próxima versão.


----
<br><del>**Update: alfa.1.0.0.2**</del>

<del>Aviso: os arquivos da versão alfa.1.0.0.0 são incompatíveis com a versão alfa.1.0.0.1 do programa.
<br>Aviso2: os arquivos da versão alfa.1.0.0.1 são compatíveis com a versão alfa.1.0.0.2, porem tem que colocar as tags: "CompressVertices:True" e "AutoScale:True" no arquivo .idxbin</del>

**Código de terceiro:**

[ObjLoader by chrisjansson](https://github.com/chrisjansson/ObjLoader/).
Encontra-se no BINrepackTest, código modificado, não considera mais a tag "g", os grupos são criados a partir da tag de material "usemtl" 

  ## BINdecoderTest

Decodifica e cria um .obj do bin.
São criados os arquivos .txt2 .obj .mtl e .idxbin
  - sendo os arquivos txt2 arquivos informativos que divide o bin por partes para melhor visualização. (agora por padrão não são gerados)
  - Agora considera o campo "IdxComp" para criar as faces corretas. (agora teoricamente todos os .obj devem estar corretos)
  - .MTL: arquivo que acompanha o .obj serve para carregar as texturas nos modelos, porem as texturas devem ser extraídas do .TPL, e enumeradas a partir de zero.
  - .IDXBIN: é o arquivo que contem o conteúdo que não esta no .obj, mas é necessário para recompilar o .BIN
  - Agora usa a escala correta presente no arquivo .bin
  - <del>Referente a orientação das faces, há alguns modelos que apresentam algumas faces invertidas, situação que ainda não foi corrigida.</del>
  - A situação acima acontece, porque, na verdade, a 2 triângulos na mesma posição, cada um com a face para um lado, porem o editor ao carregar o modelo, exibe os dois na ordem errada. (ainda encontra sem solução)
  - A classe "BINdecoder" foi restruturado, agora o bin é representado por uma classe de nome "BIN".
  - os .obj de bin de cenários que possuem cores, não geram mais o campo das normals, ja que os dados deveriam ser as normals são cores e não normals.
  - Com a opção de arquivos de debug ativo, serão gerados mais 2 arquivos .obj: ".DrawDistanceBox.obj" e "ScaleLimitBox.obj", que representam as coordenados do DrawDistance e o outro o limite da escala do objeto extraído.

## BINrepackTest

Cria um novo arquivo .BIN a partir do conteúdo do .IDXBIN e .OBJ
  - A quantidade de materiais do bin é definido pelo arquivo .obj, os materiais serão colocados em ordem alfabética no arquivo bin.
  - por exemplo: caso o arquivo obj tenha 4 materiais, no arquivo .idxbin deve conter 4 entradas cada com as tags "materialLine_*", "NodeHeader_SplitCount_*", "NodeHeader_SplitValues_*", "TopTagVifHeader_Scale_*" e "ScenarioVerticeColor_*" sendo * um número a partir de 0.
  - <del>Adicionado a compressão das vértices, que por padrão está desabilitado, que no momento se encontra em desenvolvimento, a compressão pode ocasionar a inversão de algumas faces no modelo.</del>
  - A compressão das vértices agora é funcional, deve funcionar bem, para ativar a função o arquivo .idxbin deve conter a tag "CompressVertices:True", caso não queira comprimir as vertices mude "True" para "False".
  - Adicionado auto escala, com a função ativa o programa vai determinar a escala automaticamente, aproveitando o máximo das coordenadas do "short" mantendo o objeto mais fidedigno ao original, para ativar essa função o arquivo .idxbin deve conter a tag "AutoScale:True", caso queira mudar as escalas manualmente por material mude a tag de "True" para "False".
  - Alguns pequenos bugs que crashavam o programa foram corrigidos.

At.te: JADERLINK
2023-07-22