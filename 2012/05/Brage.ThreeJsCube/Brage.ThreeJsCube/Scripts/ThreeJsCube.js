/// <reference path="Three.js" />
/// <reference path="jquery-1.7.1.min.js" />

var cubeUsingThreejs = function () {

    var container, renderer;
    var angle = 1;
    var viewPortWidth = 600;
    var viewPortHeight = 400;

    var init = function () {

        container = $('#container');
        if ($.browser.msie) {
            container.children().first().append('<div id="surface" style="height:400px; text-align:center; padding-top: 30px;"><a style="vertical-align:middle;" href="https://www.google.com/chrome">Download a proper browser</a></div>');
            return;
        }

        renderer = new THREE.WebGLRenderer();
        renderer.setSize(viewPortWidth, viewPortHeight);
        renderer.domElement.setAttribute('id', 'surface');

        var scene = new THREE.Scene();
        var camera = buildCamera();
        var cube = buildCube();

        scene.add(camera);
        scene.add(cube);
        scene.add(buildAmbientLight());
        scene.add(buildDirectionalLight());

        container.children().first().append(renderer.domElement);

        setInterval(function () {
            calculateCube(cube, scene, camera);
        }, 20);

    };

    var buildDirectionalLight = function () {
        var directionalLight = new THREE.DirectionalLight(0xffffff);
        directionalLight.position.set(1, 1, 1).normalize();
        return directionalLight;
    };

    var buildAmbientLight = function () {
        return new THREE.AmbientLight(0x999999);
    };

    var buildCube = function () {
        var cubeGeometry = new THREE.CubeGeometry(200, 200, 200, 1, 1, 1, buildMaterials());
        return new THREE.Mesh(cubeGeometry, new THREE.MeshFaceMaterial());
    };

    var buildMaterials = function () {
        var materials = [];

        var texture = new THREE.MeshLambertMaterial({
            map: THREE.ImageUtils.loadTexture('Content/me.jpg')
        });

        for (var i = 0; i < 6; i++) {
            materials.push(texture);
        }

        return materials;
    };

    var buildCamera = function () {
        var camera = new THREE.PerspectiveCamera(50, viewPortWidth / viewPortHeight, 1, 1000);
        camera.position.z = 500;
        return camera;
    };

    var degreesToRadians = function (degrees) {
        return Math.PI / 180 * degrees;
    };

    var calculateCube = function (cube, scene, camera) {
        angle++;
        cube.rotation.x = degreesToRadians(angle);
        cube.rotation.y = degreesToRadians(angle);
        cube.rotation.z = degreesToRadians(angle);
        renderer.render(scene, camera);
    };

    return {
        init: init
    };

} ().init();