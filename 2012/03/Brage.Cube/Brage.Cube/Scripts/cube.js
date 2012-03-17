
var meshCreator = function () {

    var createVertex = function (x, y, z) {
        return {
            "initialX": x,
            "initialY": y,
            "initialZ": z,
            "currentX": 0,
            "currentY": 0,
            "currentZ": 0
        };
    };

    var createPolygon = function (vA, vB, vC, vD) {
        return {
            "vertices": [vA, vB, vC, vD],
            "averageZ": null
        };
    };

    var createCubeMesh = function () {
        var mesh = { "polygons": null };
        var size = 70;

        var vertices = [];
        vertices.push(createVertex(size, size, size));
        vertices.push(createVertex(size, -size, size));
        vertices.push(createVertex(-size, size, size));
        vertices.push(createVertex(-size, -size, size));
        vertices.push(createVertex(size, size, -size));
        vertices.push(createVertex(size, -size, -size));
        vertices.push(createVertex(-size, size, -size));
        vertices.push(createVertex(-size, -size, -size));

        var polygons = mesh.polygons = [];
        polygons.push(createPolygon(vertices[0], vertices[2], vertices[3], vertices[1]));
        polygons.push(createPolygon(vertices[0], vertices[2], vertices[6], vertices[4]));
        polygons.push(createPolygon(vertices[0], vertices[1], vertices[5], vertices[4]));
        polygons.push(createPolygon(vertices[1], vertices[3], vertices[7], vertices[5]));
        polygons.push(createPolygon(vertices[3], vertices[2], vertices[6], vertices[7]));
        polygons.push(createPolygon(vertices[4], vertices[6], vertices[7], vertices[5]));

        return mesh;
    };

    return {
        createCubeMesh: createCubeMesh
    };

} ();

var matrixCalculator = function () {

    var createIdentityMatrix = function () {
        return [[1, 0, 0], [0, 1, 0], [0, 0, 1]];
    };

    var degreesToRadians = function (degrees) {
        return Math.PI / 180 * degrees;
    };

    var createXAxisRotationMatrix = function (degrees) {
        var radians = degreesToRadians(degrees);
        var matrix = createIdentityMatrix();
        matrix[1][1] = Math.cos(radians);
        matrix[1][2] = Math.sin(radians);
        matrix[2][1] = -Math.sin(radians);
        matrix[2][2] = Math.cos(radians);
        return matrix;
    };

    var createYAxisRotationMatrix = function (degrees) {
        var radians = degreesToRadians(degrees);
        var matrix = createIdentityMatrix();
        matrix[0][0] = Math.cos(radians);
        matrix[0][2] = Math.sin(radians);
        matrix[2][0] = -Math.sin(radians);
        matrix[2][2] = Math.cos(radians);
        return matrix;
    };

    var createZAxisRotationMatrix = function (degrees) {
        var radians = degreesToRadians(degrees);
        var matrix = createIdentityMatrix();
        matrix[0][0] = Math.cos(radians);
        matrix[0][1] = -Math.sin(radians);
        matrix[1][0] = Math.sin(radians);
        matrix[1][1] = Math.cos(radians);
        return matrix;
    };

    var multiplyMatrixes = function (matrix1, matrix2) {
        var matrix = createIdentityMatrix();
        for (var i = 0; i < 3; i++) {
            for (var j = 0; j < 3; j++) {
                matrix[i][j] =
                    (matrix2[i][0] * matrix1[0][j]) +
                        (matrix2[i][1] * matrix1[1][j]) +
                            (matrix2[i][2] * matrix1[2][j]);
            }
        }
        return matrix;
    };

    var createRotationMatrix = function (degreesX, degreesY, degreesZ) {
        return multiplyMatrixes(multiplyMatrixes(createXAxisRotationMatrix(degreesX), createYAxisRotationMatrix(degreesY)), createZAxisRotationMatrix(degreesZ));
    };

    var applyMatrixToVertex = function (matrix, vertex) {
        vertex.currentX = (vertex.initialX * matrix[0][0]) + (vertex.initialY * matrix[0][1]) + (vertex.initialZ * matrix[0][2]);
        vertex.currentY = (vertex.initialX * matrix[1][0]) + (vertex.initialY * matrix[1][1]) + (vertex.initialZ * matrix[1][2]);
        vertex.currentZ = (vertex.initialX * matrix[2][0]) + (vertex.initialY * matrix[2][1]) + (vertex.initialZ * matrix[2][2]);
        return vertex;
    };

    return {
        createRotationMatrix: createRotationMatrix,
        applyMatrixToVertex: applyMatrixToVertex
    };
} ();

var spinningCube = function () {

    var ctx, mesh;
    var angle = 1;
    var viewPortWidth = 600;
    var viewPortHeight = 400;
    var xPosition = viewPortWidth / 2;
    var yPosition = viewPortHeight / 2;
    var perspectiveCoefficient = 300;

    var initialize = function () {

        ctx = getInitializedContext(document.getElementById('surface'));
        mesh = meshCreator.createCubeMesh();

        setInterval(function () {
            refresh();
        }, 20);
    };

    var getInitializedContext = function (canvasElement) {
        canvasElement.width = viewPortWidth;
        canvasElement.height = viewPortHeight;
        var context = canvasElement.getContext("2d");
        return context;
    };

    var refresh = function () {
        angle++;

        calculateMesh();
        drawBackground();
        drawMesh();
    };

    var calculateMesh = function () {
        var rotationMatrix = matrixCalculator.createRotationMatrix(angle - 110, angle - 180, angle - 38);
        var polygons = mesh.polygons;

        for (var i = 0; i < polygons.length; i++) {
            for (var j = 0; j < polygons[i].vertices.length; j++) {
                var vertex = matrixCalculator.applyMatrixToVertex(rotationMatrix, polygons[i].vertices[j]);
                applyPerspective(vertex);
                applyPosition(vertex);
            }
            calculateZAverage(polygons[i]);
        }
    };

    var drawBackground = function () {
        ctx.clearRect(0, 0, viewPortWidth, viewPortHeight);
    };

    var drawMesh = function () {
        sortPolygons(mesh.polygons);
        for (var k = 0; k < mesh.polygons.length; k++) {
            drawPolygon(mesh.polygons[k]);
        }
        ctx.restore();
    };

    var drawPolygon = function (polygon) {
        var shade = calculateShade(polygon);
        ctx.beginPath();
        ctx.fillStyle = 'rgb(' + shade + ',' + shade + ',' + shade + ')';
        ctx.moveTo(polygon.vertices[0].currentX, polygon.vertices[0].currentY);
        for (var i = 1; i < polygon.vertices.length; i++) {
            ctx.lineTo(polygon.vertices[i].currentX, polygon.vertices[i].currentY);
        }
        ctx.fill();
        ctx.closePath();
    };

    var applyPosition = function (vertex) {
        vertex.currentX = vertex.currentX + xPosition;
        vertex.currentY = vertex.currentY + yPosition;
    };

    var applyPerspective = function (vertex) {
        vertex.currentX = vertex.currentX * perspectiveCoefficient / (vertex.currentZ + perspectiveCoefficient);
        vertex.currentY = vertex.currentY * perspectiveCoefficient / (vertex.currentZ + perspectiveCoefficient);
    };

    var calculateZAverage = function (polygon) {
        var zSum = 0;
        for (var i = 0; i < polygon.vertices.length; i++) {
            zSum += polygon.vertices[i].currentZ;
        }
        polygon.averageZ = zSum / polygon.vertices.length;
    };

    var calculateShade = function (polygon) {
        var shade = Math.round(Math.abs(polygon.averageZ)) * 5;

        if (shade > 255)
            shade = 255;
        if (shade < 0)
            shade = 0;

        return shade;
    };

    var sortPolygons = function (polygons) {
        return polygons.sort(function (polygon1, polygon2) {
            return polygon2.averageZ - polygon1.averageZ;
        });
    };

    return {
        initialize: initialize

    };

} ().initialize();
