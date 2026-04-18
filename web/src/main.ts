import * as THREE from "three";
import { BellmanScene } from "./scenes/BellmanScene";
import { QLearnScene } from "./scenes/QLearnScene";
import { DemoScene, ParamDescriptor } from "./scenes/Scene";
import { BOARD_HEIGHT, BOARD_WIDTH, TILE_SIZE } from "./types";

// Entry point — wires the Three.js renderer, DemoScene instances, and HTML
// control panel. Two scenes share a single renderer; switching disposes the
// old scene and instantiates the new one.

const stage = document.getElementById("stage") as HTMLDivElement;
const paramsEl = document.getElementById("params") as HTMLDivElement;
const statsEl = document.getElementById("stats") as HTMLDivElement;
const stepBtn = document.getElementById("step-btn") as HTMLButtonElement;
const autoBtn = document.getElementById("auto-btn") as HTMLButtonElement;
const resetBtn = document.getElementById("reset-btn") as HTMLButtonElement;
const bellmanBtn = document.getElementById("scene-bellman") as HTMLButtonElement;
const qlearnBtn = document.getElementById("scene-qlearn") as HTMLButtonElement;

const renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
renderer.setPixelRatio(window.devicePixelRatio);
renderer.setClearColor(0x13151a, 1);
stage.appendChild(renderer.domElement);

const scene = new THREE.Scene();
const camera = new THREE.OrthographicCamera(-1, 1, 1, -1, 0.1, 100);
camera.position.set(0, 0, 10);
camera.lookAt(0, 0, 0);

function resize() {
  const rect = stage.getBoundingClientRect();
  renderer.setSize(rect.width, rect.height, false);

  // Fit the whole grid (plus padding) into the stage, preserving aspect.
  const padding = 0.5;
  const gridW = BOARD_WIDTH * TILE_SIZE + padding * 2;
  const gridH = BOARD_HEIGHT * TILE_SIZE + padding * 2;
  const aspect = rect.width / rect.height;
  const gridAspect = gridW / gridH;

  let viewW: number, viewH: number;
  if (aspect >= gridAspect) {
    viewH = gridH;
    viewW = gridH * aspect;
  } else {
    viewW = gridW;
    viewH = gridW / aspect;
  }
  camera.left = -viewW / 2;
  camera.right = viewW / 2;
  camera.top = viewH / 2;
  camera.bottom = -viewH / 2;
  camera.updateProjectionMatrix();
}

let current: DemoScene | null = null;
let currentRoot: THREE.Object3D | null = null;

function mount(makeScene: () => DemoScene) {
  if (current) {
    if (currentRoot) scene.remove(currentRoot);
    current.dispose();
  }
  current = makeScene();
  currentRoot = current.root;
  scene.add(currentRoot);
  renderParams(current.params);
  autoBtn.textContent = current.isAuto() ? "Stop" : "Auto";
}

function renderParams(params: ParamDescriptor[]) {
  paramsEl.innerHTML = "<p><strong>Parameters</strong></p>";
  for (const p of params) {
    const row = document.createElement("div");
    row.className = "param";
    const label = document.createElement("label");
    label.textContent = p.label;
    const input = document.createElement("input");
    input.type = "number";
    input.value = String(p.value);
    if (p.min !== undefined) input.min = String(p.min);
    if (p.max !== undefined) input.max = String(p.max);
    if (p.step !== undefined) input.step = String(p.step);
    input.addEventListener("change", () => {
      const v = parseFloat(input.value);
      if (!Number.isNaN(v) && current) current.onParam(p.key, v);
    });
    row.append(label, input);
    paramsEl.append(row);
  }
}

function renderStats() {
  if (!current) return;
  const stats = current.stats();
  statsEl.innerHTML = "<p><strong>Stats</strong></p>";
  for (const [k, v] of Object.entries(stats)) {
    const row = document.createElement("div");
    row.className = "stat";
    row.innerHTML = `<span>${k}</span><span>${v}</span>`;
    statsEl.append(row);
  }
}

// Scene switching
function selectBellman() {
  bellmanBtn.classList.add("active");
  qlearnBtn.classList.remove("active");
  mount(() => new BellmanScene());
}
function selectQLearn() {
  qlearnBtn.classList.add("active");
  bellmanBtn.classList.remove("active");
  mount(() => new QLearnScene());
}
bellmanBtn.addEventListener("click", selectBellman);
qlearnBtn.addEventListener("click", selectQLearn);

// Controls
stepBtn.addEventListener("click", () => current?.step());
autoBtn.addEventListener("click", () => {
  if (!current) return;
  current.setAuto(!current.isAuto());
  autoBtn.textContent = current.isAuto() ? "Stop" : "Auto";
});
resetBtn.addEventListener("click", () => current?.reset());

// Keyboard shortcuts: Space = step, A = auto-toggle, R = reset.
window.addEventListener("keydown", e => {
  if (!current) return;
  if (e.code === "Space") { e.preventDefault(); current.step(); }
  else if (e.code === "KeyA") {
    current.setAuto(!current.isAuto());
    autoBtn.textContent = current.isAuto() ? "Stop" : "Auto";
  }
  else if (e.code === "KeyR") current.reset();
});

window.addEventListener("resize", resize);

let lastTime = performance.now();
function frame(now: number) {
  const dt = Math.min(0.1, (now - lastTime) / 1000);
  lastTime = now;
  if (current) current.tickAuto(dt);
  renderer.render(scene, camera);
  renderStats();
  requestAnimationFrame(frame);
}

selectBellman();
resize();
requestAnimationFrame(frame);
