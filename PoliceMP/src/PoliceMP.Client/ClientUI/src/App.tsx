
import * as React from 'react';
import InteractionHud from './components/overlays/InteractionHud';
import NotificationOverlay from './components/overlays/NotificationsOverlay';
import styled from 'styled-components';
import { Overlay, OverlaysContainer } from './components/core/OverlaysContainer';

const OverlayContainer = styled(OverlaysContainer)`
  display: block;
  width: auto;
  height: auto;
`

interface Props {
}

let App = (props: Props) => {
  return <>
  <OverlayContainer>

    <Overlay id="InteractionHud">
      <InteractionHud />
    </Overlay>

    <Overlay id="NewNotificationOverlay">
      <NotificationOverlay />
    </Overlay>
    
  </OverlayContainer>
</>
}

export default App;
