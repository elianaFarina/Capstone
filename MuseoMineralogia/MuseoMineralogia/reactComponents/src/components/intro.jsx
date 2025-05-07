import React from 'react'
import Carousel from 'react-bootstrap/Carousel';
import ametistImg from '../assets/ametist.jpg';
import silicatiImg from '../assets/silicates.jpg';
import mineraliMetalliciImg from '../assets/metallic-minerals.jpg';
import rocceImg from '../assets/rocks.jpg';
import gemmeImg from '../assets/gems.jpg';

export default function Intro() {
    return (
        <div className="museo-intro">
            <div className="header-section">
                <img src={ametistImg} alt="Ametista" className="header-image" />
                <h1>Museo di Mineralogia</h1>
                <p>
                    Il Museo di Mineralogia rappresenta un'importante istituzione scientifica e culturale dedicata alla conservazione, studio ed esposizione di minerali, rocce e fossili. La nostra collezione permanente ospita migliaia di esemplari mineralogici di interesse scientifico e storico, provenienti da giacimenti di tutto il mondo.
                </p>
                <p>
                    Il percorso espositivo è organizzato secondo criteri scientifici che permettono ai visitatori di comprendere le caratteristiche fisiche e chimiche dei minerali, la loro classificazione sistematica e i processi geologici che ne determinano la formazione.
                </p>
            </div>

            <Carousel className="museum-carousel">
                <Carousel.Item>
                    <img
                        className="d-block w-100"
                        src={silicatiImg}
                        alt="Sala dei Silicati"
                    />
                    <Carousel.Caption>
                        <h3>Sala dei Silicati</h3>
                        <p>Con esempi straordinari di quarzi, feldspati e gemme</p>
                    </Carousel.Caption>
                </Carousel.Item>
                <Carousel.Item>
                    <img
                        className="d-block w-100"
                        src={mineraliMetalliciImg}
                        alt="Sala dei Minerali Metallici"
                    />
                    <Carousel.Caption>
                        <h3>Sala dei Minerali Metallici</h3>
                        <p>Dedicata ai minerali di origine metallifera</p>
                    </Carousel.Caption>
                </Carousel.Item>
                <Carousel.Item>
                    <img
                        className="d-block w-100"
                        src={rocceImg}
                        alt="Sezione Petrografica"
                    />
                    <Carousel.Caption>
                        <h3>Sezione Petrografica</h3>
                        <p>Che illustra i principali tipi di rocce</p>
                    </Carousel.Caption>
                </Carousel.Item>
                <Carousel.Item>
                    <img
                        className="d-block w-100"
                        src={gemmeImg}
                        alt="Area Gemmologica"
                    />
                    <Carousel.Caption>
                        <h3>Area Gemmologica</h3>
                        <p>Con una preziosa collezione di pietre preziose</p>
                    </Carousel.Caption>
                </Carousel.Item>
            </Carousel>
        </div>
    );
}