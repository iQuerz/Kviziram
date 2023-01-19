import Card from "../components/Card";

function HomePage() {
    const imageUrl = "https://media.istockphoto.com/id/1292374537/vector/quiz-game-stage-interior-design-background-competition-with-questions-television-trivia-show.jpg?s=612x612&w=0&k=20&c=1G-zxNSromfn5IEUGbWnPNJYx0JfE6sxuv8vq_ZKJtM="
  return (
    <Card className="home-page">
      <img src={imageUrl} alt="placeholder" />
    </Card>
  );
}

export default HomePage;
