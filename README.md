# RE4-PS2-BIN-TOOL
Extract and repack ps2 re4 bin files

Translate from Portuguese Brazil

Esse é o código resultado a partir das pesquisas sobre os arquivos .bin do re4 de ps2 em conjunto com o HardRain.
Encontra-se em estágio de desenvolvimento, mas já pode ser usado para criar novos .bin para os cenários e os itens.

Update: alfa.1.0.0.1

**Aviso: os arquivos da versão alfa.1.0.0.0 são incompatíveis com a versão alfa.1.0.0.1 do programa**

**Código de terceiro:**

[ObjLoader by chrisjansson](https://github.com/chrisjansson/ObjLoader/).
Encontra-se no BINrepackTest, código modificado, não considera mais a tag "g", os grupos são criados a partir da tag de material "usemtl" 

## BINdecoderTest

Decodifica e cria um .obj do bin.
São criados os arquivos .txt2 .obj .mtl e .idxbin
  - sendo os arquivos txt2 arquivos informativos que divide o bin por partes para melhor visualização.
  - <del>.OBJ: caso o campo "IndexMount" tenha a sequência e 0x0, 0x1 e 0xFFFF, o arquivo estará correto, caso seja sempre zero estará errado, pois o programa não sabe como montar as vertices desse tipo de arquivo</del>
  - Agora considera o campo "IdxComp" para criar as faces corretas.
  - .MTL: arquivo que acompanha o .obj serve para carregar as texturas nos modelos, porem as texturas devem ser extraídas do .TPL, e enumeradas a partir de zero.
  - .IDXBIN: é o arquivo que contem o conteúdo que não esta no .obj, mas é necessário para recompilar o .BIN
  - Agora usa a escala correta presente no arquivo .bin
  - Referente a orientação das faces, há alguns modelos que apresentam algumas faces invertidas, situação que ainda não foi corrigida.
  - A classe "BINdecoder" foi restruturado, agora o bin é representado por uma classe de nome "BIN".

## BINrepackTest

Cria um novo arquivo .BIN a partir do conteúdo do .IDXBIN e .OBJ
  - A quantidade de materiais do bin é definido pelo arquivo .obj, os materiais serão colocados em ordem alfabetica no arquivo bin.
  - por exemplo: caso o arquivo obj tenha 4 materiais, no arquivo .idxbin deve conter 4 entradas cada com as tags "materialLine_*", "NodeHeader_SplitCount_*", "NodeHeader_SplitValues_*", "TopTagVifHeader_Scale_*" e "ScenarioVerticeColor_*" sendo * um número a partir de 0.
  - Adicionado a compressão das vértices, que por padrão está desabilitado, que no momento se encontra em desenvolvimento, a compressão pode ocasionar a inversão de algumas faces no modelo.
  
