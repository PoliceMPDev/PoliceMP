import * as React from 'react'
import { useState, useEffect, useContext } from 'react'
import styled from 'styled-components'
import { emitPoliceMpEvent } from '../../../hooks/usePoliceMpEvent'

const CarDashboardHudWrapper = styled.div`
    display: block;
    height: 200px;
    width: 200px;
    position: absolute;
    left: 20px;
    bottom: 40px;
`

async function testGetData() {
    const test = await emitPoliceMpEvent<string>("GetCarWindowState", null);
    console.log(test);
}

export interface CarDashboardHudProps {
}

const CarDashboardHud = (props: CarDashboardHudProps) => {

    return <CarDashboardHudWrapper>
        <button onClick={testGetData}>Get data</button>
    </CarDashboardHudWrapper>
}

export default CarDashboardHud