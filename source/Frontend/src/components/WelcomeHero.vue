<template>
  <div id="welcome">
    <h1>Welcome to <u>watch.grossweber.com</u></h1>
    <p>You will see something here as soon as your presenter starts typing.</p>
    <p>
      <strong>Please note:<br /></strong>This web page is stateless. All content will be gone if you
      refresh the page.
    </p>
  </div>
</template>

<style scoped>
#welcome {
  position: absolute;
  top: -100%;
  left: 50%;
  transform: translate(-50%, -50%);

  animation-name: drop;
  animation-delay: 1s;
  animation-fill-mode: forwards;
  animation-iteration-count: 1;
  animation-timing-function: ease-in;
  animation-duration: .5s;
}

@keyframes drop {
  to {
    top: 50%
  }
}

strong {
  font-weight: bold;
}
</style>
