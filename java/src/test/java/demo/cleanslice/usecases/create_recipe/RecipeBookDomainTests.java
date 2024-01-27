package demo.cleanslice.usecases.create_recipe;

import java.util.ArrayList;
import java.util.List;
import java.util.function.Function;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import static org.mockito.Mockito.*;
// import static org.hamcrest.MatcherAssert.*;
// import static org.hamcrest.Matchers.*;
import static org.junit.jupiter.api.Assertions.*;

class RecipeBookDomainTests {
       @SuppressWarnings("unchecked")
       Function<String, List<Recipe>> getRecipeByTitleFnMock = mock(Function.class);

       private RecipeBookDomain subject;

       final String recipeTitle = "test-title";

       @BeforeEach
       void setup() {
              subject = new RecipeBookDomain(getRecipeByTitleFnMock);
       }

       @Test
       void givenTheRecipeDoesNotExistItShouldAddItToTheRecipeBook() throws RecipeExistsException {
              when(getRecipeByTitleFnMock.apply(recipeTitle)).thenReturn(new ArrayList<Recipe>());

              subject.addRecipe(new Recipe(recipeTitle, null));

              assertEquals(1, subject.recipes.size());
              assertEquals(subject.recipes.get(0).getTitle(), recipeTitle);
       }

       @Test
       void givenTheRecipeDoesExistItShouldNotAddDuplicateToRecipeBook() {
              when(getRecipeByTitleFnMock.apply(recipeTitle)).thenReturn(List.of(new Recipe(recipeTitle, null)));

              assertThrows(RecipeExistsException.class, () -> {
                     subject.addRecipe(new Recipe(recipeTitle, null));
              });

       }
}
