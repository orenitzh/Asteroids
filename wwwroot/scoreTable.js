

//global variables
const scoreDiv = document.querySelector("div.scoreboard") 
let tableHeaders = ["Global Ranking", "Username", "Score"]


//creating the scoreboard table
const createScoreboardTable = () => {
    while (scoreDiv.firstChild) scoreDiv.removeChild(scoreDiv.firstChild) 
    let scoreboardTable = document.createElement('table') 
    scoreboardTable.className = 'scoreboardTable'
    let scoreboardTableHead = document.createElement('thead') 
    scoreboardTableHead.className = 'scoreboardTableHead'
    let scoreboardTableHeaderRow = document.createElement('tr') 
    scoreboardTableHeaderRow.className = 'scoreboardTableHeaderRow'
    tableHeaders.forEach(header => {
        let scoreHeader = document.createElement('th') 
        scoreHeader.innerText = header
        scoreboardTableHeaderRow.append(scoreHeader) 
    })
    scoreboardTableHead.append(scoreboardTableHeaderRow) 
    scoreboardTable.append(scoreboardTableHead)
    let scoreboardTableBody = document.createElement('tbody') 
    scoreboardTableBody.className = "scoreboardTable-Body"
    scoreboardTable.append(scoreboardTableBody) 
    scoreDiv.append(scoreboardTable) 
}
//get single score and its index to create the global ranking
const appendScores = (singleScore, singleScoreIndex) => {
    const scoreboardTable = document.querySelector('.scoreboardTable') 
    let scoreboardTableBodyRow = document.createElement('tr') 
    scoreboardTableBodyRow.className = 'scoreboardTableBodyRow'
    let scoreRanking = document.createElement('td')
    scoreRanking.innerText = singleScoreIndex
    let usernameData = document.createElement('td')
    usernameData.innerText = singleScore.playerName
    let scoreData = document.createElement('td')
    scoreData.innerText = singleScore.playerBestScore
    scoreboardTableBodyRow.append(scoreRanking, usernameData, scoreData) 
    scoreboardTable.append(scoreboardTableBodyRow) 
}

//get all scores from DB, returns an array of objects sorted in decreasing order
const getScores = () => {
    fetch('api/players/history') 
        .then(res => res.json())
        .then(scores => {
            //clears scoreboard div if it has any children nodes
            createScoreboardTable()
            //iterates over the scores array and appends each one to the scoretable
            for (const score of scores) {
                let scoreIndex = scores.indexOf(score) + 1 
                appendScores(score, scoreIndex) 
            }
        })
}

getScores();
