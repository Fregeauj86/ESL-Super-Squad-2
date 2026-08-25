export const STAGES = [
  { id: 'cell', name: 'Cell', humor: 'You are now a cell. Try not to panic.', moveSpeed: 150, jumpForce: 0, gravity: 220, airControl: 0.68, floatMode: true, canJump: false, canDoubleJump: false, canDash: false, color: '#6ee0c0', radius: 14 },
  { id: 'cluster', name: 'Cluster', humor: 'You split. Neither half filed the paperwork.', moveSpeed: 170, jumpForce: 240, gravity: 300, airControl: 0.72, floatMode: true, canJump: true, canDoubleJump: false, canDash: false, color: '#5fd4a8', radius: 16 },
  { id: 'organism', name: 'Organism', humor: 'Specialization unlocked. Blame biology.', moveSpeed: 176, jumpForce: 340, gravity: 500, airControl: 0.72, floatMode: true, canJump: true, canDoubleJump: false, canDash: false, color: '#8fd46a', radius: 16 },
  { id: 'primitive', name: 'Primitive', humor: 'Congratulations. You have a front and a back now.', moveSpeed: 186, jumpForce: 390, gravity: 700, airControl: 0.82, floatMode: false, canJump: true, canDoubleJump: false, canDash: false, color: '#c4b86a', radius: 17 },
  { id: 'embryo', name: 'Embryo', humor: 'Growing rapidly. Mood: squishy.', moveSpeed: 170, jumpForce: 370, gravity: 680, airControl: 0.78, floatMode: false, canJump: true, canDoubleJump: false, canDash: false, color: '#e8b8a8', radius: 17 },
  { id: 'nervous', name: 'Nervous', humor: 'Neurons online. Overthinking begins shortly.', moveSpeed: 205, jumpForce: 410, gravity: 780, airControl: 0.9, floatMode: false, canJump: true, canDoubleJump: false, canDash: false, color: '#c8b8e8', radius: 18 },
  { id: 'newborn', name: 'Newborn', humor: 'You have legs. Please use them responsibly.', moveSpeed: 215, jumpForce: 420, gravity: 800, airControl: 0.88, floatMode: false, canJump: true, canDoubleJump: false, canDash: false, color: '#f0d0c0', radius: 18 },
  { id: 'child', name: 'Child', humor: 'Double jump unlocked. Parental supervision not included.', moveSpeed: 225, jumpForce: 430, gravity: 800, airControl: 0.96, floatMode: false, canJump: true, canDoubleJump: true, canDash: false, color: '#e8dca8', radius: 18 },
  { id: 'teen', name: 'Teen', humor: 'Maximum velocity. Minimum planning.', moveSpeed: 290, jumpForce: 430, gravity: 800, airControl: 0.78, floatMode: false, canJump: true, canDoubleJump: true, canDash: true, color: '#a8c8f0', radius: 19 },
  { id: 'adult', name: 'Adult', humor: 'Fully evolved. Please use stairs like everyone else.', moveSpeed: 310, jumpForce: 460, gravity: 800, airControl: 1, floatMode: false, canJump: true, canDoubleJump: true, canDash: true, color: '#f0e4d8', radius: 20 },
];

export function getStage(index) {
  return STAGES[Math.min(Math.max(0, index), STAGES.length - 1)];
}
